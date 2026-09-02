using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

public class Ward : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}
