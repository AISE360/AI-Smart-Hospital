using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

// Represents any billable service: procedure, consultation, consumable, investigation
public class ServiceOrder : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;
    public Guid? AdmissionId { get; set; }
    public Admission? Admission { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Consultation, Procedure, Lab, Radiology, Pharmacy, Consumable, BedCharge
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice => UnitPrice * Quantity;
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public string? OrderedById { get; set; }
    public bool IsBilled { get; set; }
    public Guid? InvoiceLineId { get; set; }
    // Coding for interoperability
    public string? SnomedCode { get; set; }
    public string? LoincCode { get; set; }
    public string? CptCode { get; set; }
}
