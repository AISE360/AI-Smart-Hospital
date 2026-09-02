using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class AiOutputLog : BaseEntity
{
    public string PromptTemplate { get; set; } = string.Empty;
    public string PromptVersion { get; set; } = "v1";
    public string ModelName { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
    public string InputSummary { get; set; } = string.Empty; // de-identified summary
    public string OutputContent { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty; // Scribe, DischargeSummary, Faq, Insight, ClaimCheck
    public AiOutputStatus Status { get; set; } = AiOutputStatus.Draft;
    public string? ApprovedById { get; set; }
    public StaffUser? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public bool WasEdited { get; set; }
    public string? EditedDiff { get; set; }
    public double? ConfidenceScore { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
}
