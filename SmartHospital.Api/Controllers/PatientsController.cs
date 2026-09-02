using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Dtos;
using SmartHospital.Application.Interfaces;
using SmartHospital.Domain.Entities;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    public PatientsController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> List([FromQuery] string? search, [FromQuery] int page=1, [FromQuery] int pageSize=20)
    {
        var q = _db.Patients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => p.FullName.Contains(search) || p.Mrn.Contains(search) || (p.Phone!=null && p.Phone.Contains(search)));
        var items = await q.OrderByDescending(p=>p.CreatedAt).Skip((page-1)*pageSize).Take(pageSize)
            .Select(p=> new PatientDto(p.Id,p.Mrn,p.FullName,p.Gender.ToString(),p.DateOfBirth,p.Phone,p.Email)).ToListAsync();
        await _audit.LogAsync("Read", "Patient", "list", $"search={search}", true);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> Get(Guid id)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p==null) return NotFound();
        await _audit.LogAsync("Read", "Patient", id.ToString(), null, true);
        return new PatientDto(p.Id,p.Mrn,p.FullName,p.Gender.ToString(),p.DateOfBirth,p.Phone,p.Email);
    }

    [HttpPost]
    [Authorize(Roles="Admin,FrontDesk,Nurse")]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.FullName)) return BadRequest("FullName required");
        var gender = Enum.TryParse<Domain.Enums.Gender>(req.Gender, true, out var g) ? g : Domain.Enums.Gender.Unknown;
        var mrn = $"MRN{DateTime.UtcNow:yyyyMMdd}{new Random().Next(1000,9999)}";
        var patient = new Patient
        {
            Mrn=mrn, FullName=req.FullName, Gender=gender, DateOfBirth=req.DateOfBirth,
            Phone=req.Phone, Email=req.Email, Address=req.Address, AbhaId=req.AbhaId
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        // auto consent for treatment
        _db.ConsentRecords.Add(new ConsentRecord{ PatientId=patient.Id, Purpose="Treatment", Status=Domain.Enums.ConsentStatus.Granted});
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "Patient", patient.Id.ToString(), mrn, true);
        return CreatedAtAction(nameof(Get), new {id=patient.Id}, new PatientDto(patient.Id,patient.Mrn,patient.FullName,patient.Gender.ToString(),patient.DateOfBirth,patient.Phone,patient.Email));
    }

    [HttpGet("{id:guid}/encounters")]
    public async Task<IActionResult> Encounters(Guid id)
    {
        var list = await _db.Encounters.Where(e=>e.PatientId==id).Include(e=>e.Department).OrderByDescending(e=>e.StartTime).ToListAsync();
        return Ok(list.Select(e=> new { e.Id, e.Type, e.Status, e.StartTime, e.ChiefComplaint, Department=e.Department?.Name }));
    }
}
