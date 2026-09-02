using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BedsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public BedsController(ApplicationDbContext db){_db=db;}

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _db.Beds.Include(b=>b.Ward).Select(b=> new {
        b.Id, b.BedNumber, Ward=b.Ward.Name, WardCode=b.Ward.Code, Status=b.Status.ToString(), b.OccupiedSince, b.ExpectedDischarge
    }).ToListAsync());

    [HttpGet("occupancy")]
    public async Task<IActionResult> Occupancy()
    {
        var beds = await _db.Beds.ToListAsync();
        var total = beds.Count;
        var occupied = beds.Count(b=>b.Status==BedStatus.Occupied);
        var available = beds.Count(b=>b.Status==BedStatus.Available);
        var wards = await _db.Beds.Include(b=>b.Ward).GroupBy(b=>b.Ward.Name).Select(g=> new { ward=g.Key, total=g.Count(), occupied=g.Count(b=>b.Status==BedStatus.Occupied), occupancyPct=Math.Round((double)g.Count(b=>b.Status==BedStatus.Occupied)/g.Count()*100,1)}).ToListAsync();
        var forecast = new { expectedDischarges24h = beds.Count(b=>b.ExpectedDischarge.HasValue && b.ExpectedDischarge.Value.Date==DateTime.UtcNow.Date.AddDays(1)), expectedDischarges48h=beds.Count(b=>b.ExpectedDischarge.HasValue && b.ExpectedDischarge.Value.Date<=DateTime.UtcNow.Date.AddDays(2)) };
        return Ok(new { total, occupied, available, occupancyPct=Math.Round((double)occupied/total*100,1), wards, forecast });
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Roles="Admin,Nurse,FrontDesk")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusReq req)
    {
        var bed = await _db.Beds.FindAsync(id);
        if(bed==null) return NotFound();
        if(Enum.TryParse<BedStatus>(req.Status,true,out var st)) bed.Status=st;
        if(st==BedStatus.Occupied) bed.OccupiedSince=DateTime.UtcNow;
        if(st==BedStatus.Available){ bed.OccupiedSince=null; bed.ExpectedDischarge=null; }
        await _db.SaveChangesAsync();
        return Ok(bed);
    }

    [HttpGet("admissions")]
    public async Task<IActionResult> Admissions() => Ok(await _db.Admissions.Include(a=>a.Patient).Include(a=>a.Bed).OrderByDescending(a=>a.AdmittedAt).Take(50).Select(a=> new { a.Id, Patient=a.Patient.FullName, Bed=a.Bed.BedNumber, a.AdmittedAt, a.ExpectedDischargeAt, a.Status }).ToListAsync());

    public record StatusReq(string Status);
}
