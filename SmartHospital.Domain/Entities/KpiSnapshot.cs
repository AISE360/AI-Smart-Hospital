using SmartHospital.Domain.Common;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Domain.Entities;

public class KpiSnapshot : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    public KpiCategory Category { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty; // %, ₹, count, days
    public decimal? PreviousValue { get; set; }
    public decimal? Delta => PreviousValue.HasValue ? Value - PreviousValue.Value : null;
    public decimal? DeltaPercent => PreviousValue.HasValue && PreviousValue.Value != 0
        ? (Value - PreviousValue.Value) / PreviousValue.Value * 100 : null;
    public string? DimensionsJson { get; set; } // e.g. {"ward":"General","department":"Cardiology"}
}
