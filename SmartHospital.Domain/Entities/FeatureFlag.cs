using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

public class FeatureFlag : BaseEntity
{
    public string Key { get; set; } = string.Empty; // e.g. "module.scribe", "module.revenue"
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? EnabledForRolesJson { get; set; } // JSON array of roles
}
