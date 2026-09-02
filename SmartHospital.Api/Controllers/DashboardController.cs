using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Api.Hubs;
using SmartHospital.Application.Interfaces;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAiClient _ai;
    private readonly IHubContext<DashboardHub> _hub;
    public DashboardController(ApplicationDbContext db, IAiClient ai, IHubContext<DashboardHub> hub){_db=db;_ai=ai;_hub=hub;}

    [HttpGet("kpis")]
    public async Task<IActionResult> Kpis()
    {
        var beds = await _db.Beds.ToListAsync();
        var opdToday = await _db.Appointments.CountAsync(a=>a.ScheduledAt.Date==DateTime.UtcNow.Date);
        var admissionsToday = await _db.Admissions.CountAsync(a=>a.AdmittedAt.Date==DateTime.UtcNow.Date);
        var dischargesToday = await _db.Admissions.CountAsync(a=>a.DischargedAt.HasValue && a.DischargedAt.Value.Date==DateTime.UtcNow.Date);
        var invoices = await _db.Invoices.Where(i=>i.InvoiceDate.Date==DateTime.UtcNow.Date).SumAsync(i=> (decimal?)i.TotalAmount) ?? 0;
        var claims = await _db.Claims.ToListAsync();
        var rejectionRate = claims.Any()? (double)claims.Count(c=>c.Status==ClaimStatus.Rejected)/claims.Count*100 : 0;
        var leaks = await GetLeakCount();
        var expiry = await _db.ExpiryBatches.CountAsync(b=>b.ExpiryDate <= DateTime.UtcNow.AddDays(90));
        var snapshots = await _db.KpiSnapshots.Where(k=>k.Date==DateTime.UtcNow.Date).ToListAsync();

        var totalBeds = beds.Count;
        var occupied = beds.Count(b=>b.Status==BedStatus.Occupied);
        var occupancyPct = totalBeds==0?0: Math.Round((double)occupied/totalBeds*100,1);
        // LOS avg
        var losAdmissions = await _db.Admissions.Where(a=>a.DischargedAt!=null).ToListAsync();
        var avgLos = losAdmissions.Any()? Math.Round(losAdmissions.Average(a=> (a.DischargedAt!.Value - a.AdmittedAt).TotalDays),2):0;

        return Ok(new
        {
            bedsOccupied=occupied, bedsTotal=totalBeds, occupancyPct,
            opdToday, admissionsToday, dischargesToday, avgLos,
            revenueToday=invoices,
            outstandingClaims=claims.Where(c=>c.Status!=ClaimStatus.Approved).Sum(c=>c.ClaimedAmount),
            rejectionRate=Math.Round(rejectionRate,1),
            leakageAlerts=leaks, expiryAlerts=expiry,
            kpis=snapshots,
            generatedAt=DateTime.UtcNow
        });
    }

    [HttpGet("insight")]
    public async Task<IActionResult> Insight()
    {
        var snapshots = await _db.KpiSnapshots.Where(k=>k.Date==DateTime.UtcNow.Date).ToListAsync();
        var deltas = snapshots.Select(s=> new KpiDelta(s.MetricName, s.Value, s.PreviousValue??s.Value, s.DeltaPercent??0)).ToList();
        var insight = await _ai.GenerateDailyInsightAsync(new DailyInsightRequest(deltas, "50-bed hospital India, 70-85% occupancy, ~₹1.8cr monthly billing"));
        return Ok(new { insight, date=DateTime.UtcNow.Date, deltas });
    }

    [HttpPost("broadcast")]
    [Authorize(Roles="Admin,Management")]
    public async Task<IActionResult> Broadcast([FromBody] object payload)
    {
        await _hub.Clients.Group("kpi-watchers").SendAsync("KpiUpdated", payload);
        return Ok(new { broadcasted=true });
    }

    [HttpGet("revenue-trend")]
    public async Task<IActionResult> RevenueTrend()
    {
        var invoices = await _db.Invoices.GroupBy(i=>i.InvoiceDate.Date).Select(g=> new { date=g.Key, revenue=g.Sum(x=>x.TotalAmount), count=g.Count() }).OrderBy(x=>x.date).Take(14).ToListAsync();
        return Ok(invoices);
    }

    private async Task<int> GetLeakCount()
    {
        var sos = await _db.ServiceOrders.ToListAsync();
        var lines = await _db.InvoiceLines.ToListAsync();
        var invoices = await _db.Invoices.ToListAsync();
        var svc = new SmartHospital.Application.Services.RevenueReconciliationService();
        return svc.FindLeaks(sos,lines,invoices).Count;
    }
}
