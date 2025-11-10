using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.Quiz.Queries;

/// <summary>
/// Query to get user's quiz history
/// </summary>
public sealed record GetUserQuizHistoryQuery
{
    public Guid UserId { get; init; }
    public QuizCategory? Category { get; init; }
    public QuizDifficulty? Difficulty { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
