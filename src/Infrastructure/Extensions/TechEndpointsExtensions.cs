using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Extensions;

public  static class TechEndpointsExtensions
{
    public static void MapTechEndpoints(this WebApplication app)
    {
        app.MapGet("/api/tech", () => Results.Ok())
            .WithDisplayName("Tech")
            .WithTags("Infrastructure");
    }
}