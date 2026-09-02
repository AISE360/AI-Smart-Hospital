namespace SmartHospital.Application.Interfaces;

// Adapter per external system - hospital can plug real HMIS/LIS/RIS later
public interface IHmisAdapter
{
    Task<ExternalPatientDto?> GetPatientAsync(string externalId, CancellationToken ct = default);
    Task SyncEncounterAsync(Guid encounterId, CancellationToken ct = default);
}
public interface ILisAdapter
{
    Task PushLabOrderAsync(Guid labOrderId, CancellationToken ct = default);
    Task PullLabResultAsync(string externalResultId, CancellationToken ct = default);
}
public interface IBillingAdapter
{
    Task SyncInvoiceAsync(Guid invoiceId, CancellationToken ct = default);
}
public interface IPharmacyAdapter
{
    Task SyncStockAsync(Guid itemId, CancellationToken ct = default);
}

public record ExternalPatientDto(string ExternalId, string Name, string Gender, DateTime Dob);
