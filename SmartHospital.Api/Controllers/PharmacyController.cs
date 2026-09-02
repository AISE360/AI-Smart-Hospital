using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Application.Services;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PharmacyController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPharmacyForecastService _forecast;
    public PharmacyController(ApplicationDbContext db, IPharmacyForecastService forecast){_db=db;_forecast=forecast;}

    [HttpGet("items")]
    public async Task<IActionResult> Items()
    {
        var items = await _db.PharmacyItems.Include(p=>p.StockLevels).Include(p=>p.Batches).ToListAsync();
        return Ok(items.Select(i=> new
        {
            i.Id,i.Code,i.Name,i.Category,i.Mrp,i.CostPrice,
            stock = i.StockLevels.FirstOrDefault(),
            batches = i.Batches,
            expiryRisk = i.Batches.Count(b=>b.IsExpiryRisk),
            stockOutDays = i.StockLevels.FirstOrDefault()?.DaysOfStock
        }));
    }

    [HttpGet("expiry-alerts")]
    public async Task<IActionResult> ExpiryAlerts()
    {
        var batches = await _db.ExpiryBatches.Include(b=>b.PharmacyItem).Where(b=>b.ExpiryDate <= DateTime.UtcNow.AddDays(90) && b.Quantity>0).ToListAsync();
        return Ok(batches.Select(b=> new
        {
            b.Id, ItemName=b.PharmacyItem.Name, b.BatchNumber, b.Quantity, b.ExpiryDate,
            daysToExpiry=(b.ExpiryDate - DateTime.UtcNow).Days,
            valueAtRisk= b.Quantity * b.PharmacyItem.CostPrice
        }).OrderBy(x=>x.daysToExpiry));
    }

    [HttpGet("stockout-prediction")]
    public async Task<IActionResult> StockOut()
    {
        var levels = await _db.StockLevels.Include(s=>s.PharmacyItem).ToListAsync();
        return Ok(levels.Where(s=>s.DaysOfStock.HasValue && s.DaysOfStock<14).Select(s=> new
        {
            s.PharmacyItem.Name, s.QuantityOnHand, s.AvgDailyConsumption, s.DaysOfStock, s.PredictedStockOutDate,
            urgency = s.DaysOfStock<3? "critical" : s.DaysOfStock<7? "warning":"watch"
        }).OrderBy(x=>x.DaysOfStock));
    }

    [HttpPost("forecast/{itemId:guid}")]
    public async Task<IActionResult> Forecast(Guid itemId, [FromBody] ForecastReq req)
    {
        var item = await _db.PharmacyItems.FindAsync(itemId);
        if(item==null) return NotFound();
        // simulate daily consumption history
        var history = req.History ?? new[]{12,15,10,14,13,16,11};
        var result = _forecast.Forecast(history, 7);
        // update stock level
        var level = await _db.StockLevels.FirstOrDefaultAsync(s=>s.PharmacyItemId==itemId);
        if(level!=null)
        {
            level.AvgDailyConsumption = result.AvgDaily;
            level.PredictedStockOutDate = _forecast.PredictedStockOutDate(level.QuantityOnHand, result.AvgDaily);
            await _db.SaveChangesAsync();
        }
        return Ok(result);
    }

    [HttpGet("demand-forecast")]
    public async Task<IActionResult> DemandForecast()
    {
        var items = await _db.PharmacyItems.Include(p=>p.StockLevels).ToListAsync();
        var rnd = new Random(42);
        return Ok(items.Select(i=> {
            var hist = Enumerable.Range(0,7).Select(_=> rnd.Next(5,20)).ToArray();
            var f = _forecast.Forecast(hist);
            return new { i.Name, history=hist, forecast=f };
        }));
    }

    public record ForecastReq(int[]? History);
}
