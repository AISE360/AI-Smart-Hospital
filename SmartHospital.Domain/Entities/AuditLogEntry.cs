using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

// Immutable, append-only audit log
public class AuditLogEntry : BaseEntity
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Read, Update, Delete, Approve, Sign
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? Details { get; set; } // JSON diff or description
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSensitive { get; set; }
}
