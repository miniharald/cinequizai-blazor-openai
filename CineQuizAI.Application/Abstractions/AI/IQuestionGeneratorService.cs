using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Abstractions.AI;

/// <summary>
/// Generated question from AI
/// </summary>
public sealed class GeneratedQuestion
{
    public string Text { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
    public string? ImageUrl { get; set; }
    public string? SourceReference { get; set; }
}

/// <summary>
/// Request to generate quiz questions
/// </summary>
public sealed class QuestionGenerationRequest
{
    public TitleFactsSnapshot Snapshot { get; set; } = null!;
    public QuizDifficulty Difficulty { get; set; }
    public int QuestionCount { get; set; }
    public string LanguageCode { get; set; } = "sv-SE";
}

/// <summary>
/// Service for generating quiz questions using AI
/// </summary>
public interface IQuestionGeneratorService
{
 /// <summary>
    /// Generates quiz questions from a title snapshot using AI
    /// </summary>
    /// <param name="request">Generation request with snapshot and settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of generated questions</returns>
    Task<List<GeneratedQuestion>> GenerateQuestionsAsync(
        QuestionGenerationRequest request,
        CancellationToken cancellationToken = default);
}
