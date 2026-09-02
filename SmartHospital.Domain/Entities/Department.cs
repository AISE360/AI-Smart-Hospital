using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<StaffUser> Staff { get; set; } = new List<StaffUser>();
}
