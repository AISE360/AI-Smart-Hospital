using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class ClinicalNote : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string DoctorId { get; set; } = string.Empty;
    public StaffUser? Doctor { get; set; }
    public int Version { get; set; } = 1;
    public ClinicalNoteStatus Status { get; set; } = ClinicalNoteStatus.Draft;
    // Structured SOAP
    public string? History { get; set; }
    public string? Examination { get; set; }
    public string? Assessment { get; set; }
    public string? InvestigationOrders { get; set; }
    public string? PrescriptionDraft { get; set; }
    public string? FollowUp { get; set; }
    public string? RawTranscript { get; set; }
    // AI governance
    public bool IsAiGenerated { get; set; }
    public Guid? AiOutputLogId { get; set; }
    public AiOutputLog? AiOutputLog { get; set; }
    // Signing - immutable once signed
    public string? SignedById { get; set; }
    public StaffUser? SignedBy { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? SignatureHash { get; set; }
    public Guid? PreviousVersionId { get; set; }
    public ClinicalNote? PreviousVersion { get; set; }
}
