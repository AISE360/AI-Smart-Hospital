using SmartHospital.Application.Services;
using SmartHospital.Domain.Entities;

namespace SmartHospital.Tests;

public class RevenueReconciliationTests
{
    private readonly RevenueReconciliationService _svc = new();

    private ServiceOrder Order(Guid? encounterId=null, string code="CONS001", decimal price=600, bool billed=false, Guid? id=null)
    {
        var encId = encounterId ?? Guid.NewGuid();
        return new ServiceOrder
        {
            Id = id ?? Guid.NewGuid(),
            EncounterId = encId,
            ServiceCode = code,
            ServiceName = code,
            Category = "Consultation",
            UnitPrice = price,
            Quantity = 1,
            IsBilled = billed,
            OrderedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public void Unbilled_Service_Is_Flagged_As_Leak()
    {
        var so = Order(billed:false);
        var leaks = _svc.FindLeaks(new[] { so }, Array.Empty<InvoiceLine>(), Array.Empty<Invoice>());
        Assert.Single(leaks);
        Assert.Equal("Service performed but not billed", leaks[0].Reason);
        Assert.Equal(so.TotalPrice, leaks[0].LeakageAmount);
    }

    [Fact]
    public void Billed_Service_Is_Not_Flagged()
    {
        var so = Order(billed:true);
        // Simulate invoice line linked
        var line = new InvoiceLine { Id=Guid.NewGuid(), InvoiceId=Guid.NewGuid(), ServiceCode=so.ServiceCode, Description=so.ServiceName, Category=so.Category, UnitPrice=so.UnitPrice, Quantity=1, ServiceOrderId=so.Id };
        var leaks = _svc.FindLeaks(new[] { so }, new[] { line }, Array.Empty<Invoice>());
        Assert.Empty(leaks);
    }

    [Fact]
    public void Duplicate_Charge_Is_Flagged()
    {
        var invId = Guid.NewGuid();
        var line1 = new InvoiceLine { Id=Guid.NewGuid(), InvoiceId=invId, ServiceCode="LAB001", Description="CBC", Category="Lab", UnitPrice=400, Quantity=1 };
        var line2 = new InvoiceLine { Id=Guid.NewGuid(), InvoiceId=invId, ServiceCode="LAB001", Description="CBC", Category="Lab", UnitPrice=400, Quantity=1 };
        var leaks = _svc.FindLeaks(Array.Empty<ServiceOrder>(), new[] { line1, line2 }, Array.Empty<Invoice>());
        Assert.Single(leaks);
        Assert.Equal("Duplicate charge suspected", leaks[0].Reason);
    }

    [Fact]
    public void TotalRecoverable_Sums_Correctly()
    {
        var so1 = Order(price:600, billed:false);
        var so2 = Order(price:1200, billed:false);
        var leaks = _svc.FindLeaks(new[] { so1, so2 }, Array.Empty<InvoiceLine>(), Array.Empty<Invoice>());
        Assert.Equal(1800m, _svc.TotalRecoverable(leaks));
    }

    [Fact]
    public void Partial_Billing_Flags_Only_Unbilled()
    {
        var encId = Guid.NewGuid();
        var billed = Order(encounterId:encId, code:"CONS001", price:600, billed:true);
        var unbilled = Order(encounterId:encId, code:"LAB001", price:400, billed:false);
        var line = new InvoiceLine { Id=Guid.NewGuid(), InvoiceId=Guid.NewGuid(), ServiceCode=billed.ServiceCode, Description=billed.ServiceName, Category=billed.Category, UnitPrice=billed.UnitPrice, Quantity=1, ServiceOrderId=billed.Id };
        var leaks = _svc.FindLeaks(new[] { billed, unbilled }, new[] { line }, Array.Empty<Invoice>());
        Assert.Single(leaks);
        Assert.Equal("LAB001", leaks[0].ServiceCode);
    }

    [Fact]
    public void High_ROI_Scenario_120Days_Simulation()
    {
        // Simulate 320 IPD admissions/month, ~3.2 LOS, billing leakage 2-3%
        // For unit test: 10 encounters each with 4 services, 30% unbilled => leakage ~₹1-2L
        var rnd = new Random(42);
        var orders = new List<ServiceOrder>();
        var encId = Guid.NewGuid();
        for(int i=0;i<10;i++)
        {
            encId = Guid.NewGuid();
            for(int j=0;j<4;j++)
                orders.Add(Order(encounterId:encId, price: rnd.Next(400,2500), billed: rnd.NextDouble()>0.3));
        }
        var leaks = _svc.FindLeaks(orders, Array.Empty<InvoiceLine>(), Array.Empty<Invoice>());
        var total = _svc.TotalRecoverable(leaks);
        Assert.InRange(leaks.Count, 8, 20);
        Assert.True(total > 5000, $"Expected leakage >5k, got {total}");
    }
}
