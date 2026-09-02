using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Encounter : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public EncounterType Type { get; set; }
    public EncounterStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string? AssignedDoctorId { get; set; }
    public StaffUser? AssignedDoctor { get; set; }
    public string? ChiefComplaint { get; set; }
    // FHIR coding
    public string? SnomedCode { get; set; }
    public string? Icd10Code { get; set; }
    public string? LoincCode { get; set; }

    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
}
