using CineQuizAI.Infrastructure.ExternalServices.Tmdb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CineQuizAI.Web.Endpoints;

public static class DebugEndpoints
{
    public static void MapDebugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/debug");

        group.MapGet("/tmdb-config", ([FromServices] IOptions<TmdbConfiguration> config) =>
      {
var tmdbConfig = config.Value;

            return Results.Ok(new
            {
        BaseUrl = tmdbConfig.BaseUrl,
                ImageBaseUrl = tmdbConfig.ImageBaseUrl,
    HasApiKey = !string.IsNullOrWhiteSpace(tmdbConfig.ApiKey),
    ApiKeyPrefix = tmdbConfig.ApiKey?.Length > 20 
      ? tmdbConfig.ApiKey.Substring(0, 20) + "..." 
             : "MISSING",
       TimeoutSeconds = tmdbConfig.TimeoutSeconds
          });
      })
     .WithName("GetTmdbConfig");
    }
}
