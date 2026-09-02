using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class DischargeSummary : BaseEntity
{
    public Guid AdmissionId { get; set; }
    public Admission Admission { get; set; } = null!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public int Version { get; set; } = 1;
    public DischargeSummaryStatus Status { get; set; } = DischargeSummaryStatus.Draft;
    public string? AdmissionCourse { get; set; }
    public string? Investigations { get; set; }
    public string? Procedures { get; set; }
    public string? TreatmentGiven { get; set; }
    public string? ConditionAtDischarge { get; set; }
    public string? DischargeAdvice { get; set; }
    public string? FollowUpPlan { get; set; }
    public string? FullContent { get; set; }

    public bool IsAiGenerated { get; set; }
    public Guid? AiOutputLogId { get; set; }
    public AiOutputLog? AiOutputLog { get; set; }

    public string? SignedById { get; set; }
    public StaffUser? SignedBy { get; set; }
    public DateTime? SignedAt { get; set; }
    public Guid? PreviousVersionId { get; set; }
    public DischargeSummary? PreviousVersion { get; set; }
}
