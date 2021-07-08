using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal class WeatherForecastNotificationService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly RabbitMqPublisher _publisher;
        private readonly RabbitMqSubscriber<WeatherForecast> _subscriber;
        private readonly IWeatherForecastNotifier _weatherForecastNotifier;

        public WeatherForecastNotificationService(IHubContext<NotificationHub> hubContext,
            IWeatherForecastNotifier weatherForecastNotifier)
        {
            _hubContext = hubContext;
            _weatherForecastNotifier = weatherForecastNotifier;
            _publisher = new RabbitMqPublisher(RabbitMqOptions.RabbitExchange, RabbitConnectionFactory.GetDefault());
            _subscriber = new RabbitMqSubscriber<WeatherForecast>("web",
                RabbitMqOptions.RabbitExchange,
                RabbitMqRouting.ServiceToWeb,
                RabbitConnectionFactory.GetDefault());
            _subscriber.SetMessageHandler(MessageHandler);
        }

        public override void Dispose()
        {
            _publisher.Dispose();
            _subscriber.StopConsuming();
            _subscriber.Dispose();
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                _subscriber.StartConsuming();
                while (!_weatherForecastNotifier.IsCompleted)
                {
                    var forecast = _weatherForecastNotifier.Take();
                    _publisher.Publish(RabbitMqRouting.WebToService, forecast);
                }
            }, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => _weatherForecastNotifier.CompleteAdding(), stoppingToken);
            await base.StopAsync(stoppingToken);
        }

        private Task<bool> MessageHandler(WeatherForecast arg)
        {
            return Task.Run(() =>
            {
                _hubContext.Clients.All.SendAsync("MessageReceived", JsonSerializer.Serialize(arg));
                return true;
            });
        }
    }
}