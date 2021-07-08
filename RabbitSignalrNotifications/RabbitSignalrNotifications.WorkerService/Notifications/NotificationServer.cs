using System.Threading.Tasks;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.WorkerService.Notifications
{
    internal class NotificationServer
    {
        private readonly RabbitMqPublisher _publisher;
        private readonly RabbitMqSubscriber<WeatherForecast> _subscriber;

        public NotificationServer(RabbitMqPublisher publisher, RabbitMqSubscriber<WeatherForecast> subscriber)
        {
            _publisher = publisher;
            _subscriber = subscriber;
            _subscriber.SetMessageHandler(MessageHandler);
        }

        private Task<bool> MessageHandler(WeatherForecast forecast)
        {
            return Task.Run(() =>
            {
                _publisher.Publish(RabbitMqRouting.ServiceToWeb, forecast);
                return true;
            });
        }

        public void Start()
        {
            _subscriber.StartConsuming();
        }

        public void Stop()
        {
            _subscriber.StopConsuming();
            _subscriber.Dispose();
            _publisher.Dispose();
        }
    }
}