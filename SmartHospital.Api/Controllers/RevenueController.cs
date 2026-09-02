using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Services;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles="Admin,Billing,Management,Doctor")]
public class RevenueController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly RevenueReconciliationService _svc;
    public RevenueController(ApplicationDbContext db, RevenueReconciliationService svc){_db=db;_svc=svc;}

    [HttpGet("leaks")]
    public async Task<IActionResult> Leaks()
    {
        var sos = await _db.ServiceOrders.Include(s=>s.Encounter).ThenInclude(e=>e.Patient).ToListAsync();
        var lines = await _db.InvoiceLines.ToListAsync();
        var invoices = await _db.Invoices.ToListAsync();
        var leaks = _svc.FindLeaks(sos, lines, invoices);
        // enrich with patient name
        var enriched = leaks.Select(l =>
        {
            var so = sos.FirstOrDefault(s=>s.Id==l.ServiceOrderId);
            var patient = so?.Encounter?.Patient?.FullName ?? "Unknown";
            return new { l.ServiceOrderId, l.ServiceCode, l.ServiceName, l.LeakageAmount, l.Reason, l.Category, Patient=patient, EncounterId=so?.EncounterId };
        });
        return Ok(new { totalRecoverable=_svc.TotalRecoverable(leaks), count=leaks.Count, items=enriched });
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var sos = await _db.ServiceOrders.ToListAsync();
        var lines = await _db.InvoiceLines.ToListAsync();
        var invoices = await _db.Invoices.ToListAsync();
        var leaks = _svc.FindLeaks(sos, lines, invoices);
        var total = _svc.TotalRecoverable(leaks);
        return Ok(new
        {
            recoverableThisMonth = total,
            flaggedCount = leaks.Count,
            avgPerEncounter = leaks.Any()? total/leaks.Count : 0,
            topCategories = leaks.GroupBy(l=>l.Category).Select(g=> new { category=g.Key, amount=g.Sum(x=>x.LeakageAmount), count=g.Count() }).OrderByDescending(x=>x.amount).Take(5),
            target = "₹1.0–2.0L/month flagged",
            status = total>=100000 ? "on_track" : "attention"
        });
    }

    [HttpPost("run-reconciliation")]
    [Authorize(Roles="Admin,Billing")]
    public async Task<IActionResult> Run()
    {
        // In MVP, just return current leaks; in production this would enqueue background job via queue
        var sos = await _db.ServiceOrders.ToListAsync();
        var lines = await _db.InvoiceLines.ToListAsync();
        var invoices = await _db.Invoices.ToListAsync();
        var leaks = _svc.FindLeaks(sos, lines, invoices);
        return Ok(new { message="Reconciliation complete", leaksFound=leaks.Count, total=_svc.TotalRecoverable(leaks) });
    }
}
