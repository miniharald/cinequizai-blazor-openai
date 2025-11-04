using CineQuizAI.Application.Features.UserPreferences.Commands;
using CineQuizAI.Application.Features.UserPreferences.Handlers;
using CineQuizAI.Application.Features.UserPreferences.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CineQuizAI.Web.Endpoints;

public static class UserPreferenceEndpoints
{
    public static void MapUserPreferenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user-preferences")
            .RequireAuthorization();

        // GET /api/user-preferences/{userId}
        group.MapGet("/{userId:guid}", async (
     Guid userId,
            [FromServices] GetUserPreferenceHandler handler,
      CancellationToken ct) =>
      {
            var query = new GetUserPreferenceQuery { UserId = userId };
            var result = await handler.HandleAsync(query, ct);
     return Results.Ok(result);
     })
        .WithName("GetUserPreference")
        .Produces<CineQuizAI.Application.Features.UserPreferences.DTOs.UserPreferenceDto>(200);

     // PUT /api/user-preferences
        group.MapPut("/", async (
 [FromBody] UpdateUserPreferenceCommand command,
    [FromServices] UpdateUserPreferenceHandler handler,
     CancellationToken ct) =>
        {
   var result = await handler.HandleAsync(command, ct);
    return Results.Ok(result);
        })
        .WithName("UpdateUserPreference")
        .Produces<CineQuizAI.Application.Features.UserPreferences.DTOs.UserPreferenceDto>(200);
    }
}
