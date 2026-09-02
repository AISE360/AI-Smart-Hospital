using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class ConsentRecord : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string Purpose { get; set; } = string.Empty; // Treatment, Billing, Research, Insurance
    public ConsentStatus Status { get; set; } = ConsentStatus.Granted;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? GrantedById { get; set; }
    public string? HipId { get; set; } // ABDM HIP
    public string? ConsentArtifactId { get; set; }
}
