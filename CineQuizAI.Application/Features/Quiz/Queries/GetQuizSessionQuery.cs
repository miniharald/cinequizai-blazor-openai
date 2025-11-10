namespace CineQuizAI.Application.Features.Quiz.Queries;

/// <summary>
/// Query to get a quiz session with questions
/// </summary>
public sealed record GetQuizSessionQuery
{
    public Guid SessionId { get; init; }
}
