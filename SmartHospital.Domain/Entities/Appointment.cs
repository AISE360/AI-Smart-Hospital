using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string DoctorId { get; set; } = string.Empty;
    public StaffUser? Doctor { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 15;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? TokenNumber { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
