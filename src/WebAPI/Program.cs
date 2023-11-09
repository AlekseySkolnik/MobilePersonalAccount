using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Mvc.Controllers;

#pragma warning disable CA1852

const string serviceName = "Cabinet.WebApi";

var builder = WebApplication.CreateBuilder(args)
    .AddSerilog(serviceName) // настраиваем логирование (serilog + kibana)
    .AddHealthChecks(); // добавление HealthChecks для Prometheus
//.AddRabbitMq() -> kafka
//.AddMySql(); -> postgres

builder.Services
    .AddControllers();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger();

builder
    .AddMetrics() // настройка OpenTelemetryMetrics
    .AddTracing(serviceName); // настройка OpenTelemetryMetrics

// builder.Services
// .AddApplicationServices() добавить потом регистрацию сервисов приложения
// .AddRepositories(); добавить потом регистрацию репозиториев приложения

builder.AddCors();
var app = builder.Build();

app.UseCors();
app.UseSwaggerWithUi();
app.UseRouting();

app.UseMiddleware<LogHttpContextMiddleware>(); // логируем через Scope http context
app.UseMiddleware<HttpRequestsMetricsMiddleware>(); // используем метрики Prometheus.Metrics
app.UseMiddleware<ErrorsMiddleware>(); // глобальный обработчик ошибок в виде Middleware (второй способ через ExceptionFilter) 

app.MapHealthCheckEndpoints();
app.MapTechEndpoints();
app.UseMetrics();

app.MapControllers()
    .WithDisplayName(x =>
    {
        var descriptor = x.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
        return descriptor != null
            ? $"{descriptor.ControllerName}/{descriptor.ActionName}"
            : x.DisplayName ?? string.Empty;
    });

app.Run();