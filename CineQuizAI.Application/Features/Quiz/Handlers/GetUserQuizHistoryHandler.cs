using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.DTOs;
using CineQuizAI.Application.Features.Quiz.Queries;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for getting user's quiz history
/// </summary>
public sealed class GetUserQuizHistoryHandler
{
    private readonly IQuizSessionRepository _repository;

  public GetUserQuizHistoryHandler(IQuizSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<QuizHistoryResponseDto> HandleAsync(
    GetUserQuizHistoryQuery query,
    CancellationToken cancellationToken = default)
    {
        var (sessions, totalCount) = await _repository.GetUserHistoryAsync(
  query.UserId,
            query.Category,
 query.Difficulty,
        query.PageNumber,
            query.PageSize,
       cancellationToken);

        var items = sessions.Select(s => new QuizHistoryItemDto
        {
  Id = s.Id,
            Title = s.Title,
            Category = s.Category,
 Difficulty = s.Difficulty,
            Score = s.Score,
    MaxScore = s.QuestionCount * 10,
       QuestionCount = s.QuestionCount,
  CorrectAnswers = s.Questions.Count(q => q.Answer?.IsCorrect == true),
            StartedAt = s.StartedAt,
       FinishedAt = s.FinishedAt,
      PosterUrl = s.Snapshot.PosterUrl
        }).ToList();

        return new QuizHistoryResponseDto
        {
 Items = items,
     TotalCount = totalCount,
PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
