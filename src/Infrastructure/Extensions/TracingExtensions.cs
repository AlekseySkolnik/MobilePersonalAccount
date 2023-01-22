using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure.Extensions;

public static class TracingExtensions
{
    private static readonly Dictionary<string, object> TracingAttributes = new()
    {
        { "environment", System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty },
        { "cluster", System.Environment.GetEnvironmentVariable("CLUSTER") ?? string.Empty },
        { "hostname", Dns.GetHostName() }
    };

    public static WebApplicationBuilder AddTracing(this WebApplicationBuilder builder, string serviceName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            var samplerProbability = builder.Configuration.GetValue<double?>("OpenTelemetry:SamplerProbability");

            tracerProviderBuilder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: serviceName,
                        serviceNamespace: "cabinet",
                        autoGenerateServiceInstanceId: false)
                    .AddAttributes(TracingAttributes))
                .AddAspNetCoreInstrumentation(options => options.Filter = ExcludeHealthChecksAndMetrics)
                .AddHttpClientInstrumentation()
               // .AddSource("MySqlConnector")
               // .AddSource("MassTransit")
                .SetSampler(new TraceIdRatioBasedSampler(samplerProbability ?? 0.05))
                .AddJaegerExporter();
        });

        return builder;
    }

    private static bool ExcludeHealthChecksAndMetrics(HttpContext context) =>
        !context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase);
}