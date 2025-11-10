using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.Quiz.Commands;
using CineQuizAI.Application.Features.Quiz.DTOs;
using CineQuizAI.Application.Features.Quiz.Services;
using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Features.Quiz.Handlers;

/// <summary>
/// Handler for starting a new quiz session
/// </summary>
public sealed class StartQuizHandler
{
    private readonly IQuizSessionRepository _repository;
    private readonly ITitleFactsService _titleFactsService;
    private readonly QuestionPoolService _questionPoolService;

    public StartQuizHandler(
        IQuizSessionRepository repository,
        ITitleFactsService titleFactsService,
        QuestionPoolService questionPoolService)
    {
        _repository = repository;
        _titleFactsService = titleFactsService;
        _questionPoolService = questionPoolService;
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

        // 2. Get or generate question set
        var questionSet = await _questionPoolService.GetOrGenerateQuestionSetAsync(
            snapshot,
            command.Difficulty,
            command.QuestionCount,
            command.LanguageCode,
            command.UserId,
            cancellationToken);

        // 3. Create QuizSession
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
            QuestionCount = command.QuestionCount,
            Score = 0,
            StartedAt = DateTime.UtcNow,
            FinishedAt = null,
            Questions = new List<QuizQuestion>()
        };

        // 4. Create QuizQuestion entities from QuestionSetQuestions
        var quizQuestions = questionSet.Questions
            .OrderBy(q => q.Ordinal)
            .Take(command.QuestionCount)
            .Select(q => new QuizQuestion
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                Ordinal = q.Ordinal,
                Text = q.Text,
                Options = q.Options,
                CorrectIndex = q.CorrectIndex,
                ImageUrl = q.ImageUrl,
                SourceReference = q.SourceReference,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        // Add questions to session BEFORE saving
        foreach (var question in quizQuestions)
        {
            session.Questions.Add(question);
        }

        // Save session WITH questions in one go
        session = await _repository.CreateAsync(session, cancellationToken);

        // 5. Return DTO
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
            PosterUrl = snapshot.PosterUrl
        };
    }
}
