using System;
using System.Collections.Generic;
using System.Linq;
using RabbitSignalrNotifications.Shared;
using RabbitSignalrNotifications.Web.Notifications;

namespace RabbitSignalrNotifications.Web.Repositories
{
    internal class WeatherForecastRepo : IWeatherForecastRepo
    {
        private readonly IWeatherForecastNotifier _forecastNotifier;

        public WeatherForecastRepo(IWeatherForecastNotifier forecastNotifier)
        {
            _forecastNotifier = forecastNotifier;
            Forecasts = new List<WeatherForecast>();
            foreach (var index in Enumerable.Range(1, 5))
                Forecasts.Add(WeatherForecast.GetRandom(TimeSpan.FromDays(index)));
        }

        public IList<WeatherForecast> Forecasts { get; }

        public void AddForecast(WeatherForecast forecast)
        {
            Forecasts.Add(forecast);
            _forecastNotifier.NotifyForecastAdded(forecast);
        }
    }
}