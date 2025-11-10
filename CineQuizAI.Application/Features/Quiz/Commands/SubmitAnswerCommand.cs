namespace CineQuizAI.Application.Features.Quiz.Commands;

/// <summary>
/// Command to submit an answer to a quiz question
/// </summary>
public sealed record SubmitAnswerCommand
{
    public Guid QuestionId { get; init; }
public int SelectedIndex { get; init; }
    public int TimeSpentMs { get; init; }
}
