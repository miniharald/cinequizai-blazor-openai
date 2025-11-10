using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.DTOs;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for finishing a quiz session
/// </summary>
public sealed class FinishQuizHandler
{
    private readonly IQuizSessionRepository _sessionRepository;
    private readonly IQuizQuestionRepository _questionRepository;

    public FinishQuizHandler(
        IQuizSessionRepository sessionRepository,
        IQuizQuestionRepository questionRepository)
    {
        _sessionRepository = sessionRepository;
        _questionRepository = questionRepository;
    }

    public async Task<QuizSessionDto> HandleAsync(
        FinishQuizCommand command,
        CancellationToken cancellationToken = default)
    {
var session = await _sessionRepository.GetByIdAsync(command.SessionId, cancellationToken);

  if (session is null)
     throw new InvalidOperationException($"Quiz session {command.SessionId} not found");

        if (session.FinishedAt is not null)
            throw new InvalidOperationException("Quiz session already finished");

        // Calculate total score from all answered questions
      var totalScore = 0;
        foreach (var question in session.Questions)
   {
  if (question.Answer?.IsCorrect == true)
            {
       totalScore += 10; // 10 points per correct answer
            }
        }

     // Update session
        session.Score = totalScore;
session.FinishedAt = DateTime.UtcNow;

     await _sessionRepository.UpdateAsync(session, cancellationToken);

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
    QuestionCount = session.QuestionCount,
   Score = session.Score,
    StartedAt = session.StartedAt,
       FinishedAt = session.FinishedAt,
    PosterUrl = session.Snapshot.PosterUrl
    };
    }
}
