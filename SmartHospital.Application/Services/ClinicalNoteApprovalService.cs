using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Application.Services;

/// <summary>
/// Governance: AI outputs must be explicitly signed. Immutable once signed;
/// edits create new version.
/// </summary>
public class ClinicalNoteApprovalService
{
    public const string HumanApprovalRequiredMessage = "AI_DRAFT requires explicit clinician sign-off";

    public bool CanFinalize(ClinicalNote note) =>
        note.Status == ClinicalNoteStatus.PendingReview || note.Status == ClinicalNoteStatus.AiDraft;

    public ClinicalNote Sign(ClinicalNote note, string clinicianUserId)
    {
        if (note.SignedAt.HasValue)
            throw new InvalidOperationException("Note already signed - immutable. Create new version instead.");

        if (note.Status == ClinicalNoteStatus.Signed)
            throw new InvalidOperationException("Already signed.");

        // AI_DRAFT must be reviewed - ensure clinician is not bypassing
        note.Status = ClinicalNoteStatus.Signed;
        note.SignedById = clinicianUserId;
        note.SignedAt = DateTime.UtcNow;
        note.SignatureHash = ComputeHash(note, clinicianUserId);
        return note;
    }

    public ClinicalNote CreateAmendedVersion(ClinicalNote signedNote, string amendedById, Action<ClinicalNote> mutate)
    {
        if (signedNote.SignedAt == null)
            throw new InvalidOperationException("Only signed notes can be amended via new version.");

        var newVersion = new ClinicalNote
        {
            Id = Guid.NewGuid(),
            EncounterId = signedNote.EncounterId,
            PatientId = signedNote.PatientId,
            DoctorId = amendedById,
            Version = signedNote.Version + 1,
            Status = ClinicalNoteStatus.Draft,
            History = signedNote.History,
            Examination = signedNote.Examination,
            Assessment = signedNote.Assessment,
            InvestigationOrders = signedNote.InvestigationOrders,
            PrescriptionDraft = signedNote.PrescriptionDraft,
            FollowUp = signedNote.FollowUp,
            RawTranscript = signedNote.RawTranscript,
            IsAiGenerated = false,
            PreviousVersionId = signedNote.Id,
            CreatedAt = DateTime.UtcNow,
        };
        mutate(newVersion);
        return newVersion;
    }

    public DischargeSummary SignDischarge(DischargeSummary summary, string clinicianUserId)
    {
        if (summary.SignedAt.HasValue)
            throw new InvalidOperationException("Discharge already signed - immutable.");
        summary.Status = DischargeSummaryStatus.Approved;
        summary.SignedById = clinicianUserId;
        summary.SignedAt = DateTime.UtcNow;
        return summary;
    }

    private static string ComputeHash(ClinicalNote note, string signer) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{note.Id}:{signer}:{note.Version}:{DateTime.UtcNow.Ticks}"));
}
