using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.Quiz.DTOs;

/// <summary>
/// DTO for quiz session information
/// </summary>
public sealed record QuizSessionDto
{
  public Guid Id { get; init; }
    public Guid? UserId { get; init; }
 public QuizVisibility Visibility { get; init; }
    public QuizCategory Category { get; init; }
    public QuizDifficulty Difficulty { get; init; }
    public string LanguageCode { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
  public long TmdbId { get; init; }
    public TitleType TitleType { get; init; }
    public bool HintMode { get; init; }
    public int QuestionCount { get; init; }
    public int Score { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
    public string? PosterUrl { get; init; }
}
