namespace CineQuizAI.Application.Features.UserPreferences.Queries;

/// <summary>
/// Query to get user preferences
/// </summary>
public sealed record GetUserPreferenceQuery
{
    public Guid UserId { get; init; }
}
