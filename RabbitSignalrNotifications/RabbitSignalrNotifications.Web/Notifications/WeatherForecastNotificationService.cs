using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal interface IWeatherForecastNotifier
    {
        void NotifyForecastAdded(WeatherForecast forecast);
    }

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

    internal class WeatherForecastNotificationService : IWeatherForecastNotifier, IDisposable
    {
        private readonly RabbitMqPublisher _publisher;

        public WeatherForecastNotificationService(RabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Dispose()
        {
            _publisher.Dispose();
        }


        public void NotifyForecastAdded(WeatherForecast forecast)
        {
            _publisher.Publish(RabbitMqRouting.WebToService, forecast);
        }
    }
}