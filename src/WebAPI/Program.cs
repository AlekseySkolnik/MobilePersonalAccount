using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAPI.Infrastructure.HealthChecks;
using WebAPI.Infrastructure.HealthChecks.Npgsql;
using WebAPI.Infrastructure.JsonOptions;
using WebAPI.Infrastructure.Metrics;
using WebAPI.Infrastructure.Npgsql;
using WebAPI.Infrastructure.RabbitMq;
using WebAPI.Infrastructure.Serilog;
using WebAPI.Infrastructure.Startup;
using WebAPI.Infrastructure.Swagger;
using WebAPI.Infrastructure.Tracing;
using WebAPI.Middlewares;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args)
    .AddSerilog("cabinet")
    .AddJsonOptions()
    .AddHealthChecks(b =>
        b.AddCheck<PostgresHealthCheck>("Postgres startup health check + connection pool init",
            HealthStatus.Unhealthy, new[] { HealthCheckType.Startup }))
    .AddRabbitMq()
    .AddNpgsql();

builder.Services.AddControllers();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger();

builder
    .AddMetrics()
    .AddTracing("cabinet");

//builder.Services
//    .AddApplicationServices()
//    .AddRepositories();

builder.ValidateServiceProvider();

var app = builder.Build();

app.UseSwaggerWithUi(app.Environment);

app.UseRouting();
app.UseDodoHttpMetrics();
app.UseMiddleware<ErrorsMiddleware>();

app.MapHealthCheckEndpoints();
app.UseMetricsEndpoints();

app.MapControllers().WithDisplayName(x =>
{
    var descriptor = x.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
    return descriptor != null
        ? $"{descriptor.ControllerName}/{descriptor.ActionName}"
        : x.DisplayName ?? string.Empty;
});

app.FlushLogsOnShutdown()
    .Run();