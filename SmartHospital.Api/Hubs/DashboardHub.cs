using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmartHospital.Api.Hubs;

[Authorize]
public class DashboardHub : Hub
{
    public async Task SubscribeToKpis()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "kpi-watchers");
    }

    public async Task BroadcastKpiUpdate(object payload)
    {
        await Clients.Group("kpi-watchers").SendAsync("KpiUpdated", payload);
    }
}
