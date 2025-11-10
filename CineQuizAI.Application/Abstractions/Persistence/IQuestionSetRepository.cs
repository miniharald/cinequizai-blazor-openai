using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Abstractions.Persistence;

/// <summary>
/// Repository for QuestionSet management
/// </summary>
public interface IQuestionSetRepository
{
  /// <summary>
    /// Find active question sets for a title that user hasn't played yet
    /// </summary>
    Task<List<QuestionSet>> FindAvailableSetsAsync(
        long tmdbId,
        TitleType titleType,
        QuizDifficulty difficulty,
        string languageCode,
        string factsHash,
   int promptVersion,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new question set with questions
    /// </summary>
    Task<QuestionSet> CreateWithQuestionsAsync(
        QuestionSet questionSet,
      List<QuestionSetQuestion> questions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark that a user has played a question set
    /// </summary>
    Task MarkAsPlayedAsync(
   Guid setId,
Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increment uses count for a question set
    /// </summary>
    Task IncrementUsesCountAsync(
 Guid setId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get question set with questions by ID
    /// </summary>
    Task<QuestionSet?> GetWithQuestionsAsync(
        Guid setId,
 CancellationToken cancellationToken = default);
}
