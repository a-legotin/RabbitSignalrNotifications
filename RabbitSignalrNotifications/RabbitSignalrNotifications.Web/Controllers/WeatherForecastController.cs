using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitSignalrNotifications.Shared;
using RabbitSignalrNotifications.Web.Repositories;

namespace RabbitSignalrNotifications.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastRepo _repo;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastRepo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogDebug("Getting all forecasts");
            return _repo.Forecasts.ToArray();
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            _logger.LogDebug("Inserting forecast");
            _repo.Forecasts.Add(forecast);
            return Ok();
        }
    }
}