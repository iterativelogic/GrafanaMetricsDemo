using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GrafanaMetricsDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        int[] sleepThresholds = new int[] { 1000, 2000, 3000, 4000 };

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Activity _activity;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //using var activitySrc = new ActivitySource("");
            //activitySrc.CreateActivity("", ActivityKind.Internal, )
            //_activity.AddTag("Operation", "GET Weather Forecast");
            //_activity.AddEvent(new ActivityEvent("Staring operation GET Weather Forecast"));

            Thread.Sleep(5500);

            _activity.AddEvent(new ActivityEvent("Completed operation GET Weather Forecast"));

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        }

        [HttpGet("summary")]
        public IEnumerable<WeatherForecast> GetSummary()
        {
            Thread.Sleep(3000);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        }

        [HttpGet("duplicate")]
        public IEnumerable<WeatherForecast> GetSummaryDuplicate()
        {
            Thread.Sleep(1000);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        }


    }
}
