namespace CineQuizAI.Application.Features.Quiz.Commands;

/// <summary>
/// Command to finish a quiz session
/// </summary>
public sealed record FinishQuizCommand
{
    public Guid SessionId { get; init; }
}
