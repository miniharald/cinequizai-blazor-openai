using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineQuizAI.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for QuizSession entity
/// </summary>
public sealed class QuizSessionRepository : IQuizSessionRepository
{
    private readonly AppDbContext _context;

    public QuizSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QuizSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizSessions
   .Include(s => s.Questions)
            .Include(s => s.Snapshot)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<QuizSession> CreateAsync(QuizSession session, CancellationToken cancellationToken = default)
    {
     _context.QuizSessions.Add(session);
 await _context.SaveChangesAsync(cancellationToken);
      return session;
    }

  public async Task<QuizSession> UpdateAsync(QuizSession session, CancellationToken cancellationToken = default)
    {
   _context.QuizSessions.Update(session);
await _context.SaveChangesAsync(cancellationToken);
  return session;
    }
}
