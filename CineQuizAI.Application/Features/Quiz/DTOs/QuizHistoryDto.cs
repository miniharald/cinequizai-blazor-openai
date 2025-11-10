using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.Quiz.DTOs;

/// <summary>
/// DTO for quiz history item
/// </summary>
public sealed class QuizHistoryItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuizCategory Category { get; set; }
    public QuizDifficulty Difficulty { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public int QuestionCount { get; set; }
    public int CorrectAnswers { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? PosterUrl { get; set; }
    public int PercentageScore => MaxScore > 0 ? (int)((Score * 100.0) / MaxScore) : 0;
}

/// <summary>
/// Paginated quiz history response
/// </summary>
public sealed class QuizHistoryResponseDto
{
    public List<QuizHistoryItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
