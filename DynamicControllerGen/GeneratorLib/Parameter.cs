﻿using System.Net.Http;

namespace GeneratorLib
{
    public record Parameter(string FullTypeName, bool IsPrimitive, bool HasDefaultValue, object? DefaultValue);

    public record ActionRoute(string Name, HttpMethod Method, string Route, string? ReturnTypeName, bool hasCustomFormatter, bool IsAsync, ParameterMapping[] Mapping, ParameterMapping? Body);

    public record ControllerRoute(string Name, string? Area, string BaseRoute, ActionRoute[] Actions);
}