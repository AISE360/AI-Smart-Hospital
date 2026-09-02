namespace SmartHospital.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string entityId, string? details = null, bool isSensitive = false, CancellationToken ct = default);
}
