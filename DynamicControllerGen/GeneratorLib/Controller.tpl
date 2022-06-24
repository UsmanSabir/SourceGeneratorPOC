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

        {{ $obj = "_" + ( string.downcase ClassName ) }}

        private readonly ILogger<{{ ControllerName }}> _logger;                
        private readonly {{ ClassFullName }} {{ $obj }};


        public {{ ControllerName }}(ILogger<{{ ControllerName }}> logger, {{ ClassFullName }} obj)
        {
            _logger = logger;
            {{ $obj }} = obj;
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

                $returnString = "IActionResult"
                $actionName = action.Name
                if(action.IsAsync)
                    $returnString = "async Task<IActionResult>"
                    if $actionName | string.ends_with "Async"
                        $actionName = $actionName | string.remove "Async"
                    end
                end
            }}

        [HttpPost("{{ $actionName }}{{ $route }}")]
        public {{ $returnString }} {{ $actionName }}(
            {{-
                $indx=0
                $params=""

                for mapping in action.Mapping

                if $indx>0 
                 $params = $params + ", "
                end
                $indx=$indx + 1

                if !mapping.Parameter.IsPrimitive
                    $params = $params + " [Microsoft.AspNetCore.Mvc.FromBody] "
                else
                    $params = $params + " [Microsoft.AspNetCore.Mvc.FromRoute] "
                end

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
            System.Diagnostics.Debugger.Break();

            {{ if action.HasNoReturnType }}
            return Ok();
            {{else}}
            var result = Ok({{ if action.IsAsync }} await {{end}} {{$obj}}.{{action.Name}}(
            {{-
            $sep = ""
            for mapping in action.Mapping
            -}}
             {{$sep}}{{mapping.Key}}

             {{-
                $sep = ", ";
                end
             -}}
            ));
            return result;
            {{- end }}
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