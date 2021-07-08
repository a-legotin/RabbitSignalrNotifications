using System.Collections.Concurrent;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal class WeatherForecastNotifier : IWeatherForecastNotifier
    {
        private readonly BlockingCollection<WeatherForecast> _weatherForecasts;

        public WeatherForecastNotifier()
        {
            _weatherForecasts = new BlockingCollection<WeatherForecast>();
        }

        public void NotifyForecastAdded(WeatherForecast forecast)
        {
            _weatherForecasts.Add(forecast);
        }

        public bool IsCompleted => _weatherForecasts.IsCompleted;

        public WeatherForecast Take()
        {
            return _weatherForecasts.Take();
        }

        public void CompleteAdding()
        {
            _weatherForecasts.CompleteAdding();
        }
    }
}