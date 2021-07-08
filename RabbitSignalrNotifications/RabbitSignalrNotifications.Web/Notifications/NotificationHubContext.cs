using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal class NotificationHubContext
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubContext(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<bool> ProcessMessage(WeatherForecast forecast)
        {
            await _hubContext.Clients.All.SendAsync("MessageReceived", JsonSerializer.Serialize(forecast));
            return true;
        }
    }
}