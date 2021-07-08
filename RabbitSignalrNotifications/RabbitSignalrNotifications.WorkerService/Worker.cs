using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitSignalrNotifications.Shared;
using RabbitSignalrNotifications.WorkerService.Notifications;

namespace RabbitSignalrNotifications.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly NotificationServer _notificationServer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var publisher = new RabbitMqPublisher(RabbitMqOptions.RabbitExchange, RabbitConnectionFactory.GetDefault());
            var subscriber = new RabbitMqSubscriber<WeatherForecast>("service",
                RabbitMqOptions.RabbitExchange,
                RabbitMqRouting.WebToService,
                RabbitConnectionFactory.GetDefault());
            _notificationServer = new NotificationServer(publisher, subscriber);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _notificationServer.Start();
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}. Awaiting notifications.", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
            }
            finally
            {
                _notificationServer.Stop();
            }
        }
    }
}