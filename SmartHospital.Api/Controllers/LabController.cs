using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/lab")]
[Authorize]
public class LabController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public LabController(ApplicationDbContext db){_db=db;}

    [HttpGet("orders")]
    public async Task<IActionResult> Orders([FromQuery] string? status)
    {
        var q = _db.LabOrders.Include(l=>l.Patient).Include(l=>l.Result).AsQueryable();
        if(!string.IsNullOrEmpty(status) && Enum.TryParse<LabOrderStatus>(status,true,out var st)) q=q.Where(l=>l.Status==st);
        var list = await q.OrderByDescending(l=>l.OrderedAt).Take(100).ToListAsync();
        return Ok(list.Select(l=> new {
            l.Id, l.PatientId, Patient=l.Patient.FullName, l.TestName, l.TestCode, Status=l.Status.ToString(),
            l.OrderedAt, l.Priority, Result=l.Result?.ResultValue, Criticality=l.Result?.Criticality.ToString(),
            l.Result?.IsCritical, l.Result?.ReportedAt
        }));
    }

    [HttpPost("orders")]
    [Authorize(Roles="Doctor,Nurse,LabTechnician,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateLabReq req)
    {
        var order = new LabOrder{ PatientId=req.PatientId, EncounterId=req.EncounterId, TestCode=req.TestCode, TestName=req.TestName, Priority=req.Priority ?? "Routine", OrderedById=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value };
        _db.LabOrders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(order);
    }

    [HttpPost("results/{orderId:guid}")]
    [Authorize(Roles="LabTechnician,Doctor,Admin")]
    public async Task<IActionResult> AddResult(Guid orderId, [FromBody] ResultReq req)
    {
        var order = await _db.LabOrders.FindAsync(orderId);
        if(order==null) return NotFound();
        var criticality = Enum.TryParse<LabResultCriticality>(req.Criticality,true,out var c)? c: LabResultCriticality.Normal;
        var result = new LabResult{ LabOrderId=orderId, ResultValue=req.ResultValue, Unit=req.Unit, ReferenceRange=req.ReferenceRange, Criticality=criticality, Remarks=req.Remarks };
        // critical routing: auto-flag
        if(criticality==LabResultCriticality.Critical || criticality==LabResultCriticality.Panic)
        {
            result.IsRouted = false; // needs manual routing - alert
        }
        else result.IsRouted = true;
        _db.LabResults.Add(result);
        order.Status=LabOrderStatus.Completed;
        order.CompletedAt=DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { order, result, alert = result.IsCritical ? "CRITICAL_RESULT - routing required" : null });
    }

    [HttpGet("critical")]
    public async Task<IActionResult> Critical() => Ok(await _db.LabResults.Include(r=>r.LabOrder).ThenInclude(o=>o.Patient).Where(r=>r.IsCritical && !r.IsRouted).ToListAsync());

    [HttpGet("tat")]
    public async Task<IActionResult> Tat()
    {
        var orders = await _db.LabOrders.Where(o=>o.CompletedAt!=null).ToListAsync();
        var avgTat = orders.Any()? orders.Average(o=> (o.CompletedAt!.Value - o.OrderedAt).TotalHours):0;
        return Ok(new { avgTurnaroundHours=Math.Round(avgTat,2), totalOrders=orders.Count, byPriority=orders.GroupBy(o=>o.Priority).Select(g=> new{ priority=g.Key, count=g.Count(), avgHours=Math.Round(g.Average(x=> (x.CompletedAt!.Value - x.OrderedAt).TotalHours),2)})});
    }

    public record CreateLabReq(Guid PatientId, Guid EncounterId, string TestCode, string TestName, string? Priority);
    public record ResultReq(string ResultValue, string? Unit, string? ReferenceRange, string Criticality, string? Remarks);
}
