using Infrastructure.Extensions;
using Infrastructure.Middlewares;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args)
    .AddSerilog(); // настраиваем логирование (serilog + kibana)

builder.Services.AddControllers();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger();

builder
    .AddMetrics();

var app = builder.Build();

app.UseSwaggerWithUi();
app.UseRouting();

app.UseMiddleware<LogHttpContextMiddleware>(); // логируем через Scope http context
app.UseMiddleware<RequestsMetricsMiddleware>();
app.UseMiddleware<ErrorsMiddleware>(); // глобальный обработчик ошибок в виде Middleware (второй способ через ExceptionFilter) 

app.UseMetrics();
app.MapControllers();
app.Run();