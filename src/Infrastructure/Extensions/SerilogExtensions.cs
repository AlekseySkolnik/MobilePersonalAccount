using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Globalization;
using System.Reflection;

namespace Infrastructure.Extensions;

#pragma warning disable CA1307
#pragma warning disable CA1308
#pragma warning disable CS8602

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Host
           .UseSerilog((hostingContext, configuration) =>
           {
               configuration
                       .Enrich.FromLogContext()
                       .Enrich.WithMachineName()
                       .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment.EnvironmentName)
                       .Enrich.WithProperty("ServiceName", "Cabinet")
                       .ReadFrom.Configuration(hostingContext.Configuration);

               configuration.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
               configuration.WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture);
               configuration.WriteTo.Elasticsearch(ConfigureElasticSink(hostingContext.Configuration, hostingContext.HostingEnvironment.EnvironmentName));
               configuration.WriteTo.Async(loggerSinkConfiguration =>
                loggerSinkConfiguration.Console(
                    new ExceptionAsObjectJsonFormatter(renderMessage: true, inlineFields: true),
                        standardErrorFromLevel: LogEventLevel.Warning));
           });

        return builder;
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"] ?? "http://localhost:9200"))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower(CultureInfo.CurrentCulture).Replace(".", "-")}-{environment?.ToLower(CultureInfo.CurrentCulture).Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        };
    }
}
