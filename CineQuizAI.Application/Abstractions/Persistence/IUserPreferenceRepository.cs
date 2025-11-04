using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Abstractions.Persistence;

/// <summary>
/// Repository interface for UserPreference entity
/// </summary>
public interface IUserPreferenceRepository
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPreference> CreateAsync(UserPreference preference, CancellationToken cancellationToken = default);
    Task<UserPreference> UpdateAsync(UserPreference preference, CancellationToken cancellationToken = default);
}
