using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.DTOs;
using CineQuizAI.Application.Features.Quiz.Queries;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for getting quiz session with questions
/// </summary>
public sealed class GetQuizSessionHandler
{
    private readonly IQuizSessionRepository _repository;

    public GetQuizSessionHandler(IQuizSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<QuizSessionWithQuestionsDto?> HandleAsync(
        GetQuizSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetByIdAsync(query.SessionId, cancellationToken);

   if (session is null)
       return null;

        return new QuizSessionWithQuestionsDto
        {
            Id = session.Id,
    UserId = session.UserId,
          Title = session.Title,
            Category = session.Category,
            Difficulty = session.Difficulty,
            QuestionCount = session.QuestionCount,
    Score = session.Score,
   StartedAt = session.StartedAt,
            FinishedAt = session.FinishedAt,
          PosterUrl = session.Snapshot.PosterUrl,
      Questions = session.Questions
                .OrderBy(q => q.Ordinal)
                .Select(q => new QuizQuestionDto
  {
       Id = q.Id,
              Ordinal = q.Ordinal,
   Text = q.Text,
   Options = q.Options,
 ImageUrl = q.ImageUrl
         })
        .ToList()
        };
    }
}
