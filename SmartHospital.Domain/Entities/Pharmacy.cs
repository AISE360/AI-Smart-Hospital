using SmartHospital.Domain.Common;

namespace SmartHospital.Domain.Entities;

public class PharmacyItem : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
    public string Unit { get; set; } = "Strip";
    public decimal Mrp { get; set; }
    public decimal CostPrice { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? HsnCode { get; set; }

    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public ICollection<ExpiryBatch> Batches { get; set; } = new List<ExpiryBatch>();
}

public class StockLevel : BaseEntity
{
    public Guid PharmacyItemId { get; set; }
    public PharmacyItem PharmacyItem { get; set; } = null!;
    public string Location { get; set; } = "Main Store";
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public int Available => QuantityOnHand - QuantityReserved;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    // Forecast fields
    public double? AvgDailyConsumption { get; set; }
    public int? DaysOfStock => AvgDailyConsumption.HasValue && AvgDailyConsumption > 0
        ? (int)(QuantityOnHand / AvgDailyConsumption.Value) : null;
    public DateTime? PredictedStockOutDate { get; set; }
}

public class ExpiryBatch : BaseEntity
{
    public Guid PharmacyItemId { get; set; }
    public PharmacyItem PharmacyItem { get; set; } = null!;
    public string BatchNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ManufacturedDate { get; set; }
    public decimal CostValue => Quantity * PharmacyItem.CostPrice;
    public bool IsExpiryRisk => ExpiryDate <= DateTime.UtcNow.AddDays(90) && Quantity > 0;
    public int DaysToExpiry => (ExpiryDate - DateTime.UtcNow).Days;
}
