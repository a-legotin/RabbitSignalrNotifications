using System.Collections.Generic;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Repositories
{
    public interface IWeatherForecastRepo
    {
        IList<WeatherForecast> Forecasts { get; }
        void AddForecast(WeatherForecast forecast);
    }
}