using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Dtos;
using SmartHospital.Application.Interfaces;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAiClient _ai;
    private readonly IAuditService _audit;
    public AppointmentsController(ApplicationDbContext db, IAiClient ai, IAuditService audit){_db=db;_ai=ai;_audit=audit;}

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] DateTime? date, [FromQuery] string? doctorId)
    {
        var q = _db.Appointments.Include(a=>a.Patient).Include(a=>a.Doctor).AsQueryable();
        if (date.HasValue) q = q.Where(a=>a.ScheduledAt.Date==date.Value.Date);
        if (!string.IsNullOrEmpty(doctorId)) q = q.Where(a=>a.DoctorId==doctorId);
        var list = await q.OrderBy(a=>a.ScheduledAt).Take(100).Select(a=> new AppointmentDto(a.Id,a.PatientId,a.Patient.FullName,a.DoctorId,a.Doctor!=null?a.Doctor.FullName:"",a.ScheduledAt,a.Status.ToString(),a.TokenNumber)).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles="Admin,FrontDesk,Nurse")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest req)
    {
        var patient = await _db.Patients.FindAsync(req.PatientId);
        if (patient==null) return NotFound("Patient not found");
        var doctor = await _db.Users.FindAsync(req.DoctorId);
        if (doctor==null) return NotFound("Doctor not found");
        var token = $"T{new Random().Next(100,999)}";
        var appt = new Appointment{ PatientId=req.PatientId, DoctorId=req.DoctorId, DepartmentId=doctor.DepartmentId, ScheduledAt=req.ScheduledAt, Reason=req.Reason, TokenNumber=token, Status=AppointmentStatus.Scheduled };
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Create","Appointment",appt.Id.ToString(), token, false);
        return Ok(new AppointmentDto(appt.Id,appt.PatientId,patient.FullName,appt.DoctorId,doctor.FullName,appt.ScheduledAt,appt.Status.ToString(),appt.TokenNumber));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusUpdate req)
    {
        var appt = await _db.Appointments.FindAsync(id);
        if(appt==null) return NotFound();
        if(Enum.TryParse<AppointmentStatus>(req.Status,true,out var st)) appt.Status=st;
        if(st==AppointmentStatus.CheckedIn) appt.CheckedInAt=DateTime.UtcNow;
        if(st==AppointmentStatus.Completed) appt.CompletedAt=DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Update","Appointment",id.ToString(), req.Status, false);
        return Ok(appt);
    }

    [HttpGet("availability")]
    [AllowAnonymous]
    public async Task<IActionResult> Availability([FromQuery] string doctorId, [FromQuery] DateTime date)
    {
        // stub: return 9am-5pm slots every 15 min, mark booked
        var booked = await _db.Appointments.Where(a=>a.DoctorId==doctorId && a.ScheduledAt.Date==date.Date).Select(a=>a.ScheduledAt).ToListAsync();
        var slots = new List<object>();
        for(int h=9;h<17;h++) for(int m=0;m<60;m+=15)
        {
            var slot = date.Date.AddHours(h).AddMinutes(m);
            slots.Add(new { time=slot, available=!booked.Any(b=>Math.Abs((b-slot).TotalMinutes)<5) });
        }
        return Ok(slots);
    }

    [HttpPost("faq")]
    [AllowAnonymous]
    public async Task<ActionResult<FaqResponse>> Faq([FromBody] FaqRequest req)
    {
        var kb = "Hospital FAQ: OPD 9-5 Mon-Sat, Emergency 24x7, Booking via front desk/phone/portal, Insurance needs card+ID+preauth, Visiting 4-7pm, Discharge after consultant approval ~2hrs.";
        var ans = await _ai.GenerateFaqAnswerAsync(req.Question, kb);
        return new FaqResponse(ans, "stub-model");
    }

    public record StatusUpdate(string Status);
}
