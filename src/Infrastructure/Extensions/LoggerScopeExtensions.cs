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

        var scope = new Dictionary<string, object>
        {
            ["Scope_Method"] = context.Request.Method,
            ["Scope_Host"] = context.Request.Host,
            ["Scope_Url"] = context.Request.Path + context.Request.QueryString,
            ["Scope_Id"] = Guid.NewGuid(),
            ["Scope_CorrelationId"] = correlationId
        }.ToImmutableDictionary();

        Activity.Current?.AddBaggage("CorrelationId", correlationId);

        return logger.BeginScope(scope);
    }
}