using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Abstractions.Persistence;

/// <summary>
/// Repository for QuizQuestion and QuestionAnswer
/// </summary>
public interface IQuizQuestionRepository
{
    Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<QuestionAnswer> CreateAnswerAsync(QuestionAnswer answer, CancellationToken cancellationToken = default);
}
