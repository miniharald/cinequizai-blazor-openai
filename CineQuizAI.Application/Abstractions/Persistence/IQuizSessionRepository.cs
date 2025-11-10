using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for QuizSession entity
/// </summary>
public interface IQuizSessionRepository
{
    /// <summary>
    /// Get quiz session by ID
    /// </summary>
    Task<QuizSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new quiz session
    /// </summary>
    Task<QuizSession> CreateAsync(QuizSession session, CancellationToken cancellationToken = default);
 
    /// <summary>
    /// Update quiz session
    /// </summary>
    Task<QuizSession> UpdateAsync(QuizSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user's quiz history with pagination and filtering
    /// </summary>
    Task<(List<QuizSession> Sessions, int TotalCount)> GetUserHistoryAsync(
        Guid userId,
        QuizCategory? category,
        QuizDifficulty? difficulty,
        int pageNumber,
   int pageSize,
        CancellationToken cancellationToken = default);
}
