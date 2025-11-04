using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.Handlers;
using CineQuizAI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CineQuizAI.Web.Endpoints;

public static class QuizEndpoints
{
    public static void MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/quiz")
        .RequireAuthorization();

        // POST /api/quiz/start
        group.MapPost("/start", async (
  [FromBody] StartQuizCommand command,
     [FromServices] StartQuizHandler handler,
    CancellationToken ct) =>
   {
            var result = await handler.HandleAsync(command, ct);
   return Results.Ok(result);
        })
    .WithName("StartQuiz")
    .Produces<CineQuizAI.Application.Features.Quiz.DTOs.QuizSessionDto>(201);

        // GET /api/quiz/search/movies?query={query}&language={language}
   group.MapGet("/search/movies", async (
     [FromQuery] string query,
        [FromQuery] string language,
   [FromServices] ITmdbClient tmdbClient,
   CancellationToken ct) =>
        {
       var results = await tmdbClient.SearchMoviesAsync(query, language, ct);
        return Results.Ok(results);
        })
   .WithName("SearchMovies")
        .AllowAnonymous() // Allow search without login
        .Produces<List<CineQuizAI.Application.Abstractions.ExternalServices.TmdbSearchResult>>(200);

      // GET /api/quiz/search/tv?query={query}&language={language}
        group.MapGet("/search/tv", async (
 [FromQuery] string query,
 [FromQuery] string language,
     [FromServices] ITmdbClient tmdbClient,
    CancellationToken ct) =>
        {
            var results = await tmdbClient.SearchTvSeriesAsync(query, language, ct);
  return Results.Ok(results);
})
        .WithName("SearchTvSeries")
   .AllowAnonymous() // Allow search without login
        .Produces<List<CineQuizAI.Application.Abstractions.ExternalServices.TmdbSearchResult>>(200);
    }
}
