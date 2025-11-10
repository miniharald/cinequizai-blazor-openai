using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using CineQuizAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineQuizAI.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for QuestionSet
/// </summary>
public sealed class QuestionSetRepository : IQuestionSetRepository
{
    private readonly AppDbContext _context;

    public QuestionSetRepository(AppDbContext context)
    {
     _context = context;
    }

    public async Task<List<QuestionSet>> FindAvailableSetsAsync(
        long tmdbId,
        TitleType titleType,
      QuizDifficulty difficulty,
        string languageCode,
    string factsHash,
     int promptVersion,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Find active sets that match criteria and user hasn't played
        return await _context.QuestionSets
  .Include(s => s.Questions.OrderBy(q => q.Ordinal))
            .Where(s =>
         s.TmdbId == tmdbId &&
      s.TitleType == titleType &&
   s.Difficulty == difficulty &&
        s.LanguageCode == languageCode &&
 s.FactsHash == factsHash &&
      s.PromptVersion == promptVersion &&
     s.IsActive &&
!s.Plays.Any(p => p.UserId == userId)) // User hasn't played this set
        .OrderBy(s => s.UsesCount) // Prefer less-used sets
      .Take(5) // Limit to 5 candidates
 .ToListAsync(cancellationToken);
    }

    public async Task<QuestionSet> CreateWithQuestionsAsync(
        QuestionSet questionSet,
    List<QuestionSetQuestion> questions,
        CancellationToken cancellationToken = default)
    {
     _context.QuestionSets.Add(questionSet);
   await _context.SaveChangesAsync(cancellationToken);

      // Add questions
        foreach (var question in questions)
   {
       question.SetId = questionSet.Id;
       }

        _context.QuestionSetQuestions.AddRange(questions);
        await _context.SaveChangesAsync(cancellationToken);

        return questionSet;
    }

    public async Task MarkAsPlayedAsync(
        Guid setId,
 Guid userId,
   CancellationToken cancellationToken = default)
    {
     var play = new UserQuestionSetPlay
   {
            UserId = userId,
     SetId = setId,
      PlayedAt = DateTime.UtcNow
      };

 _context.UserQuestionSetPlays.Add(play);
    await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task IncrementUsesCountAsync(
        Guid setId,
        CancellationToken cancellationToken = default)
    {
     var set = await _context.QuestionSets.FindAsync(new object[] { setId }, cancellationToken);
        if (set is not null)
        {
 set.UsesCount++;
  await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<QuestionSet?> GetWithQuestionsAsync(
        Guid setId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuestionSets
    .Include(s => s.Questions.OrderBy(q => q.Ordinal))
      .FirstOrDefaultAsync(s => s.Id == setId, cancellationToken);
    }
}
