using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middlewares;

public class LogHttpContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogHttpContextMiddleware> _logger;

    public LogHttpContextMiddleware(RequestDelegate next, ILogger<LogHttpContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        using (_logger.BeginScopeWith(context))
        {
            await _next.Invoke(context);
        }
    }
}