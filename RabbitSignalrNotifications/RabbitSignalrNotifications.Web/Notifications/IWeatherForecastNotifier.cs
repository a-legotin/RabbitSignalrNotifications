using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal interface IWeatherForecastNotifier
    {
        bool IsCompleted { get; }
        void NotifyForecastAdded(WeatherForecast forecast);
        WeatherForecast Take();
        void CompleteAdding();
    }
}