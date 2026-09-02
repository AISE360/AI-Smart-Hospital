using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Claim : BaseEntity
{
    public string ClaimNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string PayerName { get; set; } = string.Empty; // TPA/Insurer
    public string? TpaCode { get; set; }
    public decimal ClaimedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.NotSubmitted;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Icd10Code { get; set; }
    public string? ProcedureCode { get; set; }

    public ICollection<ClaimFlag> Flags { get; set; } = new List<ClaimFlag>();
    public ICollection<DenialRecord> Denials { get; set; } = new List<DenialRecord>();
}

public class ClaimFlag : BaseEntity
{
    public Guid ClaimId { get; set; }
    public Claim Claim { get; set; } = null!;
    public ClaimFlagType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium"; // Low/Medium/High/Critical
    public ClaimFlagStatus Status { get; set; } = ClaimFlagStatus.Open;
    public string? AssignedToId { get; set; }
    public StaffUser? AssignedTo { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class DenialRecord : BaseEntity
{
    public Guid ClaimId { get; set; }
    public Claim Claim { get; set; } = null!;
    public string PayerName { get; set; } = string.Empty;
    public string DenialReason { get; set; } = string.Empty;
    public string DenialCode { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal DeniedAmount { get; set; }
    public DateTime DeniedAt { get; set; } = DateTime.UtcNow;
    public string? CorrectiveAction { get; set; }
}
