using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Repositories
{
    internal class WeatherForecastRepo : IWeatherForecastRepo
    {
       

        public WeatherForecastRepo()
        {
            Forecasts = new List<WeatherForecast>();
              foreach (var index in Enumerable.Range(1, 5))
              {
                  Forecasts.Add(WeatherForecast.GetRandom(TimeSpan.FromDays(index)));
              }
        }

        public IList<WeatherForecast> Forecasts { get; } 
    }
}