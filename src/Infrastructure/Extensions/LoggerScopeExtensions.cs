using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Extensions;

public static class LoggerScopeExtensions
{
    public static IDisposable? BeginScopeWith<T>(this ILogger<T> logger, HttpContext context)
    {
        var correlationId = context.TraceIdentifier;
        var r = new Random();

        var scope = new Dictionary<string, object>
        {
            ["Method"] = context.Request.Method,
            ["Host"] = context.Request.Host,
            ["Url"] = context.Request.Path + context.Request.QueryString,
#pragma warning disable CA5394
            ["BeginScope"] = r.NextInt64()
#pragma warning restore CA5394
        }.ToImmutableDictionary();

        Activity.Current?.AddBaggage("CorrelationId", correlationId);

        return logger.BeginScope(scope);
    }
}