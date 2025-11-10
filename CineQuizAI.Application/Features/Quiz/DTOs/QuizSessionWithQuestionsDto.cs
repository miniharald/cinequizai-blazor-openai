using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.Quiz.DTOs;

/// <summary>
/// DTO for quiz session with full question details
/// </summary>
public sealed class QuizSessionWithQuestionsDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuizCategory Category { get; set; }
    public QuizDifficulty Difficulty { get; set; }
    public int QuestionCount { get; set; }
    public int Score { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? PosterUrl { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}
