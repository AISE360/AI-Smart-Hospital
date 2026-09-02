using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Bed : BaseEntity
{
    public string BedNumber { get; set; } = string.Empty;
    public Guid WardId { get; set; }
    public Ward Ward { get; set; } = null!;
    public BedStatus Status { get; set; } = BedStatus.Available;
    public string? CurrentPatientId { get; set; }
    public DateTime? OccupiedSince { get; set; }
    public DateTime? ExpectedDischarge { get; set; }
}
