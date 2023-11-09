using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;

namespace Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string serviceName)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Host
            .UseSerilog((hostingContext, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                    .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
                    .WriteTo.Elasticsearch(ConfigureElasticSink(hostingContext.Configuration,
                        hostingContext.HostingEnvironment.EnvironmentName))
                    .WriteTo.Async(loggerSinkConfiguration =>
                        loggerSinkConfiguration.Console(
                            new ExceptionAsObjectJsonFormatter(renderMessage: true, inlineFields: true),
                            standardErrorFromLevel: LogEventLevel.Information))
                    .ReadFrom.Configuration(hostingContext.Configuration);
            });

        return builder;
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CA1307 // Specify StringComparison for clarity
        return new ElasticsearchSinkOptions(
            new Uri(configuration["ElasticConfiguration:Uri"] ?? "elasticsearch:9200"))
        {
            AutoRegisterTemplate = true,
            IndexFormat =
                $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower(CultureInfo.CurrentCulture).Replace(".", "-")}-{environment?.ToLower(CultureInfo.CurrentCulture).Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        };
#pragma warning restore CA1307 // Specify StringComparison for clarity
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}