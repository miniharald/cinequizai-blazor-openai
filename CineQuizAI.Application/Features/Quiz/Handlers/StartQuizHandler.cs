using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.DTOs;
using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for starting a new quiz session
/// </summary>
public sealed class StartQuizHandler
{
    private readonly IQuizSessionRepository _repository;
    private readonly ITitleFactsService _titleFactsService;

    public StartQuizHandler(IQuizSessionRepository repository, ITitleFactsService titleFactsService)
    {
        _repository = repository;
        _titleFactsService = titleFactsService;
    }

    public async Task<QuizSessionDto> HandleAsync(
        StartQuizCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch or create TitleFactsSnapshot
        var snapshot = await _titleFactsService.GetOrCreateSnapshotAsync(
 command.TmdbId,
         command.TitleType,
     command.LanguageCode,
   cancellationToken);

  // 2. Create QuizSession
 var session = new QuizSession
        {
       Id = Guid.NewGuid(),
  UserId = command.UserId,
Visibility = command.Visibility,
        Category = command.Category,
     Difficulty = command.Difficulty,
   LanguageCode = command.LanguageCode,
         Title = snapshot.Title,
    TmdbId = command.TmdbId,
            TitleType = command.TitleType,
 SnapshotId = snapshot.Id,
  HintMode = command.HintMode,
       QuestionCount = command.QuestionCount,
   Score = 0,
         StartedAt = DateTime.UtcNow,
   FinishedAt = null
        };

      session = await _repository.CreateAsync(session, cancellationToken);

        // 3. Return DTO (questions will be generated in next step)
 return new QuizSessionDto
        {
       Id = session.Id,
  UserId = session.UserId,
     Visibility = session.Visibility,
    Category = session.Category,
   Difficulty = session.Difficulty,
      LanguageCode = session.LanguageCode,
       Title = session.Title,
 TmdbId = session.TmdbId,
   TitleType = session.TitleType,
     HintMode = session.HintMode,
   QuestionCount = session.QuestionCount,
            Score = session.Score,
     StartedAt = session.StartedAt,
  FinishedAt = session.FinishedAt,
     PosterUrl = snapshot.PosterUrl
   };
    }
}
