using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class LabOrder : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? LoincCode { get; set; }
    public LabOrderStatus Status { get; set; } = LabOrderStatus.Ordered;
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public string? OrderedById { get; set; }
    public DateTime? CollectedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Priority { get; set; } // Routine, Urgent, Stat
    public LabResult? Result { get; set; }
}

public class LabResult : BaseEntity
{
    public Guid LabOrderId { get; set; }
    public LabOrder LabOrder { get; set; } = null!;
    public string ResultValue { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public LabResultCriticality Criticality { get; set; } = LabResultCriticality.Normal;
    public bool IsCritical => Criticality == LabResultCriticality.Critical || Criticality == LabResultCriticality.Panic;
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public string? ReportedById { get; set; }
    public string? Remarks { get; set; }
    public bool IsRouted { get; set; }
    public DateTime? RoutedAt { get; set; }
}
