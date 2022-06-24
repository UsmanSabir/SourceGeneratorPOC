using System.Net.Http;

namespace GeneratorLib
{
    public record Parameter(string FullTypeName, bool IsPrimitive, bool HasDefaultValue, object? DefaultValue);

    public record ActionRoute(string Name, HttpMethod Method, string Route, string? ReturnTypeName, bool HasCustomFormatter, bool HasNoReturnType, bool IsAsync, ParameterMapping[] Mapping, ParameterMapping? Body);

    public record ControllerRoute(string Name, string ClassName, string ClassFullName, string? Area, string BaseRoute, ActionRoute[] Actions);
}