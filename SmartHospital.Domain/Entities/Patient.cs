using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Patient : BaseEntity
{
    // FHIR-style: identifier, name, gender, birthDate
    public string Mrn { get; set; } = string.Empty; // Medical Record Number
    public string FullName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AadhaarHash { get; set; } // hashed, not plain
    public string? AbhaId { get; set; } // ABDM Health ID
    // Coding fields for interoperability
    public string? Icd10Code { get; set; }
    public string? SnomedCode { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<ConsentRecord> Consents { get; set; } = new List<ConsentRecord>();
}
