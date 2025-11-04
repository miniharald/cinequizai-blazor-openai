namespace CineQuizAI.Application.Features.Quiz.DTOs;

/// <summary>
/// DTO for quiz question
/// </summary>
public sealed record QuizQuestionDto
{
    public Guid Id { get; init; }
    public int Ordinal { get; init; }
    public string Text { get; init; } = string.Empty;
    public List<string> Options { get; init; } = new();
    public string? ImageUrl { get; init; }
}
