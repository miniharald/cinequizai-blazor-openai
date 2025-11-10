using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.Handlers;
using CineQuizAI.Application.Features.Quiz.Queries;
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

    // GET /api/quiz/{sessionId}
      group.MapGet("/{sessionId:guid}", async (
   Guid sessionId,
 [FromServices] GetQuizSessionHandler handler,
    CancellationToken ct) =>
     {
      var query = new GetQuizSessionQuery { SessionId = sessionId };
var result = await handler.HandleAsync(query, ct);

   if (result is null)
   return Results.NotFound();

    return Results.Ok(result);
  })
        .WithName("GetQuizSession")
     .Produces<CineQuizAI.Application.Features.Quiz.DTOs.QuizSessionWithQuestionsDto>(200)
  .Produces(404);

 // POST /api/quiz/answer
        group.MapPost("/answer", async (
[FromBody] SubmitAnswerCommand command,
 [FromServices] SubmitAnswerHandler handler,
   CancellationToken ct) =>
        {
    var result = await handler.HandleAsync(command, ct);
   return Results.Ok(result);
        })
        .WithName("SubmitAnswer")
        .Produces<CineQuizAI.Application.Features.Quiz.DTOs.AnswerResultDto>(200);

        // POST /api/quiz/finish
   group.MapPost("/finish", async (
     [FromBody] FinishQuizCommand command,
[FromServices] FinishQuizHandler handler,
   CancellationToken ct) =>
        {
 var result = await handler.HandleAsync(command, ct);
return Results.Ok(result);
 })
.WithName("FinishQuiz")
    .Produces<CineQuizAI.Application.Features.Quiz.DTOs.QuizSessionDto>(200);

        // GET /api/quiz/history?userId={userId}&category={category}&difficulty={difficulty}&pageNumber={pageNumber}&pageSize={pageSize}
   group.MapGet("/history", async (
  [FromQuery] Guid userId,
       [FromQuery] QuizCategory? category,
            [FromQuery] QuizDifficulty? difficulty,
  [FromQuery] int pageNumber,
 [FromQuery] int pageSize,
  [FromServices] GetUserQuizHistoryHandler handler,
            CancellationToken ct) =>
     {
   var query = new GetUserQuizHistoryQuery
  {
     UserId = userId,
   Category = category,
      Difficulty = difficulty,
     PageNumber = pageNumber > 0 ? pageNumber : 1,
       PageSize = pageSize > 0 ? pageSize : 20
 };

   var result = await handler.HandleAsync(query, ct);
  return Results.Ok(result);
        })
   .WithName("GetQuizHistory")
  .Produces<CineQuizAI.Application.Features.Quiz.DTOs.QuizHistoryResponseDto>(200);

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
   .AllowAnonymous()
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
   .AllowAnonymous()
        .Produces<List<CineQuizAI.Application.Abstractions.ExternalServices.TmdbSearchResult>>(200);
    }
}
