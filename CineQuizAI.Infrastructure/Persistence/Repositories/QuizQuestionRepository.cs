using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineQuizAI.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for QuizQuestion
/// </summary>
public sealed class QuizQuestionRepository : IQuizQuestionRepository
{
    private readonly AppDbContext _context;

    public QuizQuestionRepository(AppDbContext context)
    {
        _context = context;
  }

    public async Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizQuestions
   .Include(q => q.Answer)
      .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<QuestionAnswer> CreateAnswerAsync(
     QuestionAnswer answer,
        CancellationToken cancellationToken = default)
    {
  _context.QuestionAnswers.Add(answer);
        await _context.SaveChangesAsync(cancellationToken);
        return answer;
    }
}
