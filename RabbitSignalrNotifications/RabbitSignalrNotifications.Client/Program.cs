using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitSignalrNotifications.Shared;

namespace RabbitSignalrNotifications.Client
{
    internal class Program
    {
        private static readonly string _serviceUrl = "http://localhost:5000";
        private static readonly Uri _forecastUri = new("WeatherForecast", UriKind.Relative);

        private static async Task Main(string[] args)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine("Getting forecasts");
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_serviceUrl);
                    httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders
                        .Add("User", Guid.NewGuid().ToString());

                    var response = await httpClient.GetAsync(_forecastUri);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed: {response.StatusCode}");
                        return;
                    }

                    var forecasts =
                        JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync());
                    Console.WriteLine($"Got {forecasts.Length} forecasts");
                    Console.WriteLine("Posting new forecast");
                    var newForecast =
                        WeatherForecast.GetRandom(TimeSpan.FromDays(new Random(DateTime.Now.Millisecond).Next(1, 100)));
                    // var content = new StringContent(JsonSerializer.Serialize(newForecast), Encoding.UTF8, "application/json");

                    var postResponse = await httpClient.PostAsync(_forecastUri,
                        JsonContent.Create(newForecast, typeof(WeatherForecast)));
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed: {postResponse.StatusCode}");
                        return;
                    }

                    Console.WriteLine("Posted new forecast");
                }
            });
        }
    }
}