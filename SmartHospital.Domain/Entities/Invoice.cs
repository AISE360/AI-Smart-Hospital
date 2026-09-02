using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }
    public Guid? AdmissionId { get; set; }
    public Admission? Admission { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public string? CreatedById { get; set; }
    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}

public class InvoiceLine : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public string ServiceCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
    public Guid? ServiceOrderId { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }
}
