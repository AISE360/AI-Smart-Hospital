using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Interfaces;
using SmartHospital.Application.Services;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DischargeSummariesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAiClient _ai;
    private readonly ClinicalNoteApprovalService _approval;
    private readonly IAuditService _audit;
    public DischargeSummariesController(ApplicationDbContext db, IAiClient ai, ClinicalNoteApprovalService approval, IAuditService audit){_db=db;_ai=ai;_approval=approval;_audit=audit;}

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? admissionId)
    {
        var q = _db.DischargeSummaries.AsQueryable();
        if(admissionId.HasValue) q=q.Where(d=>d.AdmissionId==admissionId);
        return Ok(await q.OrderByDescending(d=>d.CreatedAt).Take(50).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _db.DischargeSummaries.FindAsync(id));

    [HttpPost("generate")]
    [Authorize(Roles="Doctor,Admin")]
    public async Task<IActionResult> Generate([FromBody] GenerateReq req)
    {
        var adm = await _db.Admissions.Include(a=>a.Patient).Include(a=>a.Encounter).FirstOrDefaultAsync(a=>a.Id==req.AdmissionId);
        if(adm==null) return NotFound("Admission not found");
        var encounter = adm.Encounter;
        var serviceOrders = await _db.ServiceOrders.Where(s=>s.EncounterId==encounter.Id).ToListAsync();
        var investigations = await _db.LabOrders.Where(l=>l.EncounterId==encounter.Id).ToListAsync();

        var admCourse = req.AdmissionCourse ?? $"Admitted for {adm.Diagnosis}. LOS {(DateTime.UtcNow - adm.AdmittedAt).Days} days.";
        var invJson = System.Text.Json.JsonSerializer.Serialize(investigations.Select(i=> new {i.TestName,i.Status}));
        var procJson = System.Text.Json.JsonSerializer.Serialize(serviceOrders.Where(s=>s.Category=="Procedure").Select(s=>s.ServiceName));

        var aiRes = await _ai.GenerateDischargeSummaryAsync(new DischargeSummaryAiRequest(admCourse, invJson, procJson, adm.Patient.FullName));
        var aiLog = new AiOutputLog{ PromptTemplate="discharge-v1", PromptVersion="v1", ModelName="stub-model", ModelVersion="1.0", InputSummary=admCourse, OutputContent=aiRes.FullContent, TaskType="DischargeSummary", Status=AiOutputStatus.Draft, EntityType="DischargeSummary" };
        _db.AiOutputLogs.Add(aiLog);
        await _db.SaveChangesAsync();

        var summary = new DischargeSummary{
            AdmissionId=adm.Id, PatientId=adm.PatientId,
            AdmissionCourse=aiRes.AdmissionCourse, Investigations=aiRes.Investigations, Procedures=aiRes.Procedures,
            TreatmentGiven=aiRes.TreatmentGiven, ConditionAtDischarge=aiRes.ConditionAtDischarge,
            DischargeAdvice=aiRes.DischargeAdvice, FollowUpPlan=aiRes.FollowUpPlan, FullContent=aiRes.FullContent,
            IsAiGenerated=true, AiOutputLogId=aiLog.Id, Status=DischargeSummaryStatus.AiDraft, Version=1
        };
        _db.DischargeSummaries.Add(summary);
        aiLog.EntityId = summary.Id.ToString();
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AiGenerate","DischargeSummary",summary.Id.ToString(), aiLog.Id.ToString(), true);
        return Ok(new { summary, badge="AI_DRAFT" });
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles="Doctor,Admin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var s = await _db.DischargeSummaries.FindAsync(id);
        if(s==null) return NotFound();
        if(s.SignedAt.HasValue) return BadRequest("Already approved - immutable");
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        _approval.SignDischarge(s, userId);
        if(s.AiOutputLogId.HasValue)
        {
            var log = await _db.AiOutputLogs.FindAsync(s.AiOutputLogId);
            if(log!=null){ log.Status=AiOutputStatus.Approved; log.ApprovedById=userId; log.ApprovedAt=DateTime.UtcNow; }
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Approve","DischargeSummary",s.Id.ToString(), userId, true);
        return Ok(s);
    }

    public record GenerateReq(Guid AdmissionId, string? AdmissionCourse);
}
