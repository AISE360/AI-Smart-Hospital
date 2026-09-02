using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Interfaces;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClaimsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAiClient _ai;
    public ClaimsController(ApplicationDbContext db, IAiClient ai){_db=db;_ai=ai;}

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _db.Claims.Include(c=>c.Flags).OrderByDescending(c=>c.CreatedAt).Take(100).ToListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _db.Claims.Include(c=>c.Flags).FirstOrDefaultAsync(c=>c.Id==id));

    [HttpPost]
    [Authorize(Roles="Billing,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateClaimReq req)
    {
        var inv = await _db.Invoices.FindAsync(req.InvoiceId);
        if(inv==null) return NotFound("Invoice not found");
        var claim = new Claim{ ClaimNumber=$"CLM-{new Random().Next(10000,99999)}", InvoiceId=req.InvoiceId, PatientId=inv.PatientId, PayerName=req.PayerName, ClaimedAmount=req.ClaimedAmount, Icd10Code=req.Icd10Code, ProcedureCode=req.ProcedureCode, Status=ClaimStatus.NotSubmitted };
        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();
        return Ok(claim);
    }

    [HttpPost("{id:guid}/precheck")]
    public async Task<IActionResult> PreCheck(Guid id)
    {
        var claim = await _db.Claims.Include(c=>c.Invoice).FirstOrDefaultAsync(c=>c.Id==id);
        if(claim==null) return NotFound();
        var docs = new List<string>{ "Invoice" };
        // check if discharge exists
        var hasDischarge = await _db.DischargeSummaries.AnyAsync(d=>d.PatientId==claim.PatientId);
        if(hasDischarge) docs.Add("DischargeSummary");
        // simulate preauth doc presence
        if(claim.PayerName.Contains("Star")) docs.Add("PreAuth");

        var result = await _ai.PreCheckClaimAsync(new ClaimPreCheckRequest(claim.PayerName, claim.ClaimedAmount, claim.Icd10Code, claim.ProcedureCode, docs));
        // create flags for issues
        foreach(var issue in result.Issues)
        {
            _db.ClaimFlags.Add(new ClaimFlag{ ClaimId=claim.Id, Type=ClaimFlagType.MissingDocument, Description=issue, Severity="High", Status=ClaimFlagStatus.Open });
        }
        foreach(var w in result.Warnings)
        {
            _db.ClaimFlags.Add(new ClaimFlag{ ClaimId=claim.Id, Type=ClaimFlagType.PayerSpecific, Description=w, Severity="Medium", Status=ClaimFlagStatus.Open });
        }
        await _db.SaveChangesAsync();
        return Ok(new { claimId=claim.Id, result.Passed, Issues=result.Issues, Warnings=result.Warnings, flagsCreated=result.Issues.Count+result.Warnings.Count });
    }

    [HttpGet("denials/analytics")]
    public async Task<IActionResult> DenialAnalytics()
    {
        var denials = await _db.DenialRecords.ToListAsync();
        var byPayer = denials.GroupBy(d=>d.PayerName).Select(g=> new { payer=g.Key, count=g.Count(), amount=g.Sum(x=>x.DeniedAmount), avg=g.Average(x=> (double)x.DeniedAmount) });
        var byReason = denials.GroupBy(d=>d.DenialReason).Select(g=> new { reason=g.Key, count=g.Count() });
        var byDept = denials.GroupBy(d=>d.Department).Select(g=> new { department=g.Key, count=g.Count() });
        var trend = denials.GroupBy(d=>d.DeniedAt.Date).Select(g=> new { date=g.Key, count=g.Count() }).OrderBy(x=>x.date).Take(14);
        return Ok(new { totalDenials=denials.Count, totalDeniedAmount=denials.Sum(d=>d.DeniedAmount), byPayer, byReason, byDept, trend });
    }

    [HttpPatch("flags/{flagId:guid}")]
    [Authorize(Roles="Billing,Admin")]
    public async Task<IActionResult> UpdateFlag(Guid flagId, [FromBody] FlagUpdate req)
    {
        var flag = await _db.ClaimFlags.FindAsync(flagId);
        if(flag==null) return NotFound();
        if(Enum.TryParse<ClaimFlagStatus>(req.Status,true,out var st)) flag.Status=st;
        if(!string.IsNullOrEmpty(req.AssignedToId)) flag.AssignedToId=req.AssignedToId;
        await _db.SaveChangesAsync();
        return Ok(flag);
    }

    public record CreateClaimReq(Guid InvoiceId, string PayerName, decimal ClaimedAmount, string? Icd10Code, string? ProcedureCode);
    public record FlagUpdate(string Status, string? AssignedToId);
}
