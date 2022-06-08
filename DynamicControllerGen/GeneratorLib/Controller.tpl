using Microsoft.AspNetCore.Mvc;

namespace {{ NameSpace }}
{
    [ApiController]
    [Route("[controller]")]
    public class {{ ControllerName }} : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<{{ ControllerName }}> _logger;

        public {{ ControllerName }}(ILogger<{{ ControllerName }}> logger)
        {
            _logger = logger;
        }

        {{- for action in Actions }}
        [HttpPost("{{ action.Name }}")]
        public IActionResult {{ action.Name }}()
        {      
            System.Diagnostics.Debugger.Launch();
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray());
        }
        {{- end }}
    }
}