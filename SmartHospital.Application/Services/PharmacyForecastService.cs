namespace SmartHospital.Application.Services;

/// <summary>
/// Simple moving-average forecast behind IForecast interface, swappable for ML later.
/// </summary>
public interface IPharmacyForecastService
{
    ForecastResult Forecast(int[] dailyConsumption, int daysAhead = 7);
    int? DaysUntilStockOut(int quantityOnHand, double avgDailyConsumption);
    DateTime? PredictedStockOutDate(int quantityOnHand, double avgDailyConsumption);
}

public record ForecastResult(double AvgDaily, double PredictedNextWeek, double TrendSlope, string Method);

public class MovingAverageForecastService : IPharmacyForecastService
{
    public ForecastResult Forecast(int[] dailyConsumption, int daysAhead = 7)
    {
        if (dailyConsumption.Length == 0)
            return new ForecastResult(0, 0, 0, "moving_average");

        var avg = dailyConsumption.Average();
        // Simple linear regression slope
        var n = dailyConsumption.Length;
        var xAvg = (n - 1) / 2.0;
        var yAvg = avg;
        double num = 0, den = 0;
        for (int i = 0; i < n; i++)
        {
            num += (i - xAvg) * (dailyConsumption[i] - yAvg);
            den += (i - xAvg) * (i - xAvg);
        }
        var slope = den == 0 ? 0 : num / den;
        var predicted = avg * daysAhead + slope * daysAhead; // naive trend

        return new ForecastResult(Math.Round(avg, 2), Math.Round(Math.Max(0, predicted), 2), Math.Round(slope, 4), "moving_average");
    }

    public int? DaysUntilStockOut(int quantityOnHand, double avgDailyConsumption)
    {
        if (avgDailyConsumption <= 0) return null;
        return (int)Math.Floor(quantityOnHand / avgDailyConsumption);
    }

    public DateTime? PredictedStockOutDate(int quantityOnHand, double avgDailyConsumption)
    {
        var days = DaysUntilStockOut(quantityOnHand, avgDailyConsumption);
        return days.HasValue ? DateTime.UtcNow.AddDays(days.Value) : null;
    }
}
