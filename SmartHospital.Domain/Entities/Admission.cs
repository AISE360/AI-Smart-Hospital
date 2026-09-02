using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Admission : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;
    public Guid BedId { get; set; }
    public Bed Bed { get; set; } = null!;
    public DateTime AdmittedAt { get; set; }
    public DateTime? ExpectedDischargeAt { get; set; }
    public DateTime? DischargedAt { get; set; }
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Admitted;
    public string? AdmittingDoctorId { get; set; }
    public StaffUser? AdmittingDoctor { get; set; }
    public string? Diagnosis { get; set; }
    public string? Icd10Code { get; set; }
    public DischargeSummary? DischargeSummary { get; set; }
}
