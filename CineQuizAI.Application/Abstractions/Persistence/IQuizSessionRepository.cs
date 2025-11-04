using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for QuizSession entity
/// </summary>
public interface IQuizSessionRepository
{
    Task<QuizSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<QuizSession> CreateAsync(QuizSession session, CancellationToken cancellationToken = default);
    Task<QuizSession> UpdateAsync(QuizSession session, CancellationToken cancellationToken = default);
}
