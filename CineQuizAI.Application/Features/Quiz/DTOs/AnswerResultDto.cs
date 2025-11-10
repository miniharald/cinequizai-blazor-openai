namespace CineQuizAI.Application.Features.Quiz.DTOs;

/// <summary>
/// DTO for answer submission result
/// </summary>
public sealed class AnswerResultDto
{
    public Guid QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public int CorrectIndex { get; set; }
    public int PointsEarned { get; set; }
}
