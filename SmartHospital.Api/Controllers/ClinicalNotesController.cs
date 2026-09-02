using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Dtos;
using SmartHospital.Application.Interfaces;
using SmartHospital.Application.Services;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClinicalNotesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IAiClient _ai;
    private readonly ClinicalNoteApprovalService _approval;
    private readonly IAuditService _audit;
    public ClinicalNotesController(ApplicationDbContext db, IAiClient ai, ClinicalNoteApprovalService approval, IAuditService audit){_db=db;_ai=ai;_approval=approval;_audit=audit;}

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? encounterId, [FromQuery] Guid? patientId)
    {
        var q = _db.ClinicalNotes.Include(c=>c.AiOutputLog).AsQueryable();
        if(encounterId.HasValue) q=q.Where(c=>c.EncounterId==encounterId);
        if(patientId.HasValue) q=q.Where(c=>c.PatientId==patientId);
        var list = await q.OrderByDescending(c=>c.CreatedAt).Take(50).Select(c=> new ClinicalNoteDto(c.Id,c.EncounterId,c.PatientId,c.Version,c.Status.ToString(),c.History,c.Assessment,c.IsAiGenerated,c.SignedById,c.SignedAt)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var n = await _db.ClinicalNotes.Include(c=>c.AiOutputLog).FirstOrDefaultAsync(c=>c.Id==id);
        if(n==null) return NotFound();
        return Ok(n);
    }

    [HttpPost("draft")]
    [Authorize(Roles="Doctor,Nurse,Admin")]
    public async Task<IActionResult> CreateDraft([FromBody] CreateClinicalNoteRequest req)
    {
        var enc = await _db.Encounters.FindAsync(req.EncounterId);
        if(enc==null) return NotFound("Encounter not found");
        var note = new ClinicalNote{
            EncounterId=req.EncounterId, PatientId=req.PatientId, DoctorId=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "",
            History=req.History, Examination=req.Examination, Assessment=req.Assessment,
            InvestigationOrders=req.InvestigationOrders, PrescriptionDraft=req.PrescriptionDraft, FollowUp=req.FollowUp,
            RawTranscript=req.RawTranscript, Status=ClinicalNoteStatus.Draft, IsAiGenerated=false, Version=1
        };
        _db.ClinicalNotes.Add(note);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Create","ClinicalNote",note.Id.ToString(), "Draft", true);
        return Ok(note);
    }

    [HttpPost("{id:guid}/generate-ai")]
    [Authorize(Roles="Doctor,Admin")]
    public async Task<IActionResult> GenerateAi(Guid id, [FromBody] GenerateRequest req)
    {
        var note = await _db.ClinicalNotes.FindAsync(id);
        if(note==null) return NotFound();
        if(note.SignedAt.HasValue) return BadRequest("Cannot regenerate AI for signed note - create amended version.");

        var transcript = req.Transcript ?? note.RawTranscript ?? $"{note.History} {note.Assessment}";
        var aiResult = await _ai.GenerateClinicalNoteAsync(new ClinicalNoteAiRequest(transcript, $"Patient {note.PatientId}", "General Medicine"));

        // Log AI output
        var aiLog = new AiOutputLog{
            PromptTemplate="scribe-v1", PromptVersion="v1", ModelName="stub-model", ModelVersion="1.0",
            InputSummary= transcript.Length>200? transcript.Substring(0,200): transcript,
            OutputContent= aiResult.RawOutput, TaskType="Scribe", Status=AiOutputStatus.Draft, EntityType="ClinicalNote", EntityId=note.Id.ToString()
        };
        _db.AiOutputLogs.Add(aiLog);
        await _db.SaveChangesAsync();

        note.History = req.Overwrite ? aiResult.History : note.History ?? aiResult.History;
        note.Examination = aiResult.Examination;
        note.Assessment = aiResult.Assessment;
        note.InvestigationOrders = aiResult.InvestigationOrders;
        note.PrescriptionDraft = aiResult.PrescriptionDraft;
        note.FollowUp = aiResult.FollowUp;
        note.IsAiGenerated = true;
        note.AiOutputLogId = aiLog.Id;
        note.Status = ClinicalNoteStatus.AiDraft;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AiGenerate","ClinicalNote",note.Id.ToString(), aiLog.Id.ToString(), true);
        return Ok(new { note, aiLog, badge="AI_DRAFT" });
    }

    [HttpPost("{id:guid}/sign")]
    [Authorize(Roles="Doctor,Admin")]
    public async Task<IActionResult> Sign(Guid id)
    {
        var note = await _db.ClinicalNotes.FindAsync(id);
        if(note==null) return NotFound();
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            _approval.Sign(note, userId);
            // also update AI log to approved
            if(note.AiOutputLogId.HasValue)
            {
                var log = await _db.AiOutputLogs.FindAsync(note.AiOutputLogId.Value);
                if(log!=null){ log.Status=AiOutputStatus.Approved; log.ApprovedById=userId; log.ApprovedAt=DateTime.UtcNow; }
            }
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Sign","ClinicalNote",note.Id.ToString(), $"signedBy={userId} v{note.Version}", true);
            return Ok(new { message="Signed successfully - immutable", note });
        }
        catch(InvalidOperationException ex){ return BadRequest(new { error=ex.Message }); }
    }

    [HttpPost("{id:guid}/amend")]
    [Authorize(Roles="Doctor,Admin")]
    public async Task<IActionResult> Amend(Guid id, [FromBody] CreateClinicalNoteRequest req)
    {
        var existing = await _db.ClinicalNotes.FindAsync(id);
        if(existing==null) return NotFound();
        if(existing.SignedAt==null) return BadRequest("Only signed notes can be amended - edit draft instead");
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var amended = _approval.CreateAmendedVersion(existing, userId, n =>
        {
            if(req.History!=null) n.History=req.History;
            if(req.Assessment!=null) n.Assessment=req.Assessment;
            if(req.PrescriptionDraft!=null) n.PrescriptionDraft=req.PrescriptionDraft;
        });
        _db.ClinicalNotes.Add(amended);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Amend","ClinicalNote",amended.Id.ToString(), $"prev={existing.Id}", true);
        return Ok(amended);
    }

    public record GenerateRequest(string? Transcript, bool Overwrite=true);
}
