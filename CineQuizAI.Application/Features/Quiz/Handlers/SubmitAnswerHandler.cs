using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.DTOs;
using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for submitting quiz answer
/// </summary>
public sealed class SubmitAnswerHandler
{
    private readonly IQuizQuestionRepository _questionRepository;

    public SubmitAnswerHandler(IQuizQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<AnswerResultDto> HandleAsync(
  SubmitAnswerCommand command,
        CancellationToken cancellationToken = default)
    {
   var question = await _questionRepository.GetByIdAsync(command.QuestionId, cancellationToken);

 if (question is null)
  throw new InvalidOperationException($"Question {command.QuestionId} not found");

  // Check if already answered
        if (question.Answer is not null)
        throw new InvalidOperationException("Question has already been answered");

  // Calculate correctness
        var isCorrect = command.SelectedIndex == question.CorrectIndex;

   // Calculate points (simple scoring: 10 points per correct answer)
   var pointsEarned = isCorrect ? 10 : 0;

        // Create answer entity
        var answer = new QuestionAnswer
        {
    Id = Guid.NewGuid(),
      QuestionId = command.QuestionId,
    SelectedIndex = command.SelectedIndex,
  IsCorrect = isCorrect,
          TimeSpentMs = command.TimeSpentMs,
 AnsweredAt = DateTime.UtcNow
        };

        await _questionRepository.CreateAnswerAsync(answer, cancellationToken);

   return new AnswerResultDto
 {
       QuestionId = command.QuestionId,
    IsCorrect = isCorrect,
      CorrectIndex = question.CorrectIndex,
    PointsEarned = pointsEarned
        };
    }
}
