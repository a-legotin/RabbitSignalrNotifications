using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Client
{
    class Program
    {
        private static string _serviceUrl = "http://localhost:5000";
        private static Uri _forecastUri = new Uri("WeatherForecast", UriKind.Relative);
        static async Task Main(string[] args)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine("Getting forecasts");
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_serviceUrl);
                    var response = await httpClient.GetAsync(_forecastUri);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        return;
                    }
                    var forecasts =
                        JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());
                    Console.WriteLine($"Got {forecasts.Length} forecasts");
                    Console.WriteLine($"Posting new forecast");
                    var newForecast = WeatherForecast.GetRandom(TimeSpan.FromDays(new Random(DateTime.Now.Millisecond).Next(1, 100)));

                    var postResponse = await httpClient.PostAsync(_forecastUri,
                        new StringContent(JsonSerializer.Serialize(newForecast)));
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        return;
                    }
                    Console.WriteLine("Posted new forecast");
                }
            });
        }
    }
}