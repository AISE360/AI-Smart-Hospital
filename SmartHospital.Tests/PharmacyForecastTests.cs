using SmartHospital.Application.Services;

namespace SmartHospital.Tests;

public class PharmacyForecastTests
{
    private readonly IPharmacyForecastService _svc = new MovingAverageForecastService();

    [Fact]
    public void Forecast_Average_Correct()
    {
        var r = _svc.Forecast(new[]{10,12,14,12,10}, 7);
        Assert.Equal(11.6, r.AvgDaily, 1);
        Assert.Equal("moving_average", r.Method);
    }

    [Fact]
    public void DaysUntilStockOut_Calculation()
    {
        var days = _svc.DaysUntilStockOut(100, 10);
        Assert.Equal(10, days);
    }

    [Fact]
    public void PredictedStockOut_Null_When_No_Consumption()
    {
        Assert.Null(_svc.DaysUntilStockOut(100, 0));
        Assert.Null(_svc.PredictedStockOutDate(100, 0));
    }

    [Fact]
    public void Trend_Slope_Positive_When_Increasing()
    {
        var r = _svc.Forecast(new[]{5,10,15,20}, 7);
        Assert.True(r.TrendSlope > 0);
    }
}
