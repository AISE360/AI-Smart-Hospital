using Microsoft.AspNetCore.Identity;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

// IdentityUser with additional hospital fields
public class StaffUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public StaffRole Role { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string? EmployeeId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SnomedCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool MfaEnabled { get; set; }
    public string? MfaSecret { get; set; }
}

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}
