using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.Quiz.Commands;

/// <summary>
/// Command to start a new quiz session
/// </summary>
public sealed record StartQuizCommand
{
    public Guid UserId { get; init; }
 public long TmdbId { get; init; }
  public TitleType TitleType { get; init; }
    public QuizCategory Category { get; init; }
    public QuizDifficulty Difficulty { get; init; }
    public string LanguageCode { get; init; } = "sv-SE";
    public QuizVisibility Visibility { get; init; } = QuizVisibility.Public;
    public int QuestionCount { get; init; } = 5;
}
