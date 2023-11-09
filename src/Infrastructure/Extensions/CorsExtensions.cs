using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCors(this WebApplicationBuilder builder)
    {
        var origins = builder.Configuration.GetSection($"CorsConfiguration:Url")
            ?.Get<string>()
            ?.Split(',')
            ?.Where(x => !string.IsNullOrEmpty(x))
            ?.ToArray();

        return builder.Services.AddCors(options => options.AddDefaultPolicy(
            b =>
            {
                if (origins == null || origins.Length == 0)
                {
                    b.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials();
                }
                else
                {
                    b.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins(origins);
                }
            }));
    }
}