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
        {{-
                $route=""

                for mapping in action.Mapping
                
                    if !mapping.Parameter.IsPrimitive
                        continue
                    end

                    $nullChr =""
                    if mapping.Parameter.HasDefaultValue
                        $nullChr ="?"
                    end

                    $route = $route + "/{" + mapping.Key + $nullChr + "}" 
                end
            }}

        [HttpPost("{{ action.Name }}{{ $route }}")]
        public IActionResult {{ action.Name }}(
            {{-
                $indx=0
                $params=""

                for mapping in action.Mapping

                if $indx>0 
                 $params = $params + ", "
                end
                $indx=$indx + 1

                $nullChr =""
                if mapping.Parameter.HasDefaultValue
                    $nullChr ="?"
                end
                $params = $params + mapping.Parameter.FullTypeName + $nullChr
                $params = $params + " " + mapping.Key
                if mapping.Parameter.HasDefaultValue
                    $params = $params + " = " + mapping.Parameter.DefaultValue
                end

                end
                $indx=0
            -}}

            {{- $params -}}
        )
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