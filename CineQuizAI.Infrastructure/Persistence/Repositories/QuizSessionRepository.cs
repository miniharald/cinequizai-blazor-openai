using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
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

    public async Task<(List<QuizSession> Sessions, int TotalCount)> GetUserHistoryAsync(
 Guid userId,
 QuizCategory? category,
        QuizDifficulty? difficulty,
        int pageNumber,
        int pageSize,
  CancellationToken cancellationToken = default)
    {
        var query = _context.QuizSessions
            .Include(s => s.Snapshot)
     .Include(s => s.Questions)
   .ThenInclude(q => q.Answer)
       .Where(s => s.UserId == userId && s.FinishedAt != null);

     // Apply filters
        if (category.HasValue)
        {
     query = query.Where(s => s.Category == category.Value);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(s => s.Difficulty == difficulty.Value);
    }

 // Get total count
   var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
   var sessions = await query
   .OrderByDescending(s => s.FinishedAt)
        .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

   return (sessions, totalCount);
    }
}
