using Microsoft.AspNetCore.Http;
using SmartHospital.Application.Interfaces;
using SmartHospital.Domain.Entities;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;

    public AuditService(ApplicationDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    public async Task LogAsync(string action, string entityType, string entityId, string? details = null, bool isSensitive = false, CancellationToken ct = default)
    {
        var ctx = _http.HttpContext;
        var entry = new AuditLogEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = ctx?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            UserName = ctx?.User?.Identity?.Name,
            UserRole = ctx?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ctx?.Connection?.RemoteIpAddress?.ToString(),
            UserAgent = ctx?.Request?.Headers.UserAgent.ToString(),
            IsSensitive = isSensitive
        };
        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync(ct);
    }
}
