using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineQuizAI.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for UserPreference entity
/// </summary>
public sealed class UserPreferenceRepository : IUserPreferenceRepository
{
    private readonly AppDbContext _context;

    public UserPreferenceRepository(AppDbContext context)
    {
      _context = context;
    }

  public async Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
     return await _context.UserPreferences
      .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<UserPreference> CreateAsync(UserPreference preference, CancellationToken cancellationToken = default)
 {
        _context.UserPreferences.Add(preference);
        await _context.SaveChangesAsync(cancellationToken);
     return preference;
    }

    public async Task<UserPreference> UpdateAsync(UserPreference preference, CancellationToken cancellationToken = default)
    {
    _context.UserPreferences.Update(preference);
  await _context.SaveChangesAsync(cancellationToken);
   return preference;
    }
}
