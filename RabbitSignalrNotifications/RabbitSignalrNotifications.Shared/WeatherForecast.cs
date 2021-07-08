using System;

namespace RabbitSignalrNotifications.Shared
{
    public class WeatherForecast
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        public string Summary { get; set; }

        public static WeatherForecast GetRandom(TimeSpan timeOffset)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            return new WeatherForecast
            {
                Date = DateTime.Now.AddMilliseconds(-timeOffset.TotalMilliseconds),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }
    }
}