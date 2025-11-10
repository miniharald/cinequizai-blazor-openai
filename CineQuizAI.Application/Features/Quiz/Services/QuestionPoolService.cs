using CineQuizAI.Application.Abstractions.AI;
using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CineQuizAI.Application.Features.Quiz.Services;

/// <summary>
/// Service for managing question pool and generation
/// </summary>
public sealed class QuestionPoolService
{
    private readonly IQuestionSetRepository _questionSetRepository;
    private readonly IQuestionGeneratorService _questionGenerator;
    private readonly ILogger<QuestionPoolService> _logger;

    private const int CurrentPromptVersion = 1;
    private const int MaxUsesPerSet = 100;

    public QuestionPoolService(
        IQuestionSetRepository questionSetRepository,
        IQuestionGeneratorService questionGenerator,
    ILogger<QuestionPoolService> logger)
 {
      _questionSetRepository = questionSetRepository;
        _questionGenerator = questionGenerator;
 _logger = logger;
    }

    /// <summary>
 /// Get or generate a question set for a quiz session
    /// </summary>
    public async Task<QuestionSet> GetOrGenerateQuestionSetAsync(
        TitleFactsSnapshot snapshot,
        QuizDifficulty difficulty,
        int questionCount,
        string languageCode,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Calculate facts hash
        var factsHash = CalculateFactsHash(snapshot);

 // Try to find an existing suitable question set
        var availableSets = await _questionSetRepository.FindAvailableSetsAsync(
   snapshot.TmdbId,
          snapshot.TitleType,
        difficulty,
        languageCode,
      factsHash,
  CurrentPromptVersion,
            userId,
            cancellationToken);

        // Filter sets that haven't exceeded max uses
   var usableSet = availableSets
        .Where(s => s.UsesCount < MaxUsesPerSet)
     .OrderBy(s => s.UsesCount)
          .FirstOrDefault();

    if (usableSet is not null)
        {
            _logger.LogInformation(
       "Reusing existing question set {SetId} for {Title} (uses: {Uses})",
            usableSet.Id,
        snapshot.Title,
     usableSet.UsesCount);

// Mark as played by this user
    await _questionSetRepository.MarkAsPlayedAsync(usableSet.Id, userId, cancellationToken);

      // Increment uses count
         await _questionSetRepository.IncrementUsesCountAsync(usableSet.Id, cancellationToken);

            return usableSet;
        }

        // No suitable set found, generate new questions
        _logger.LogInformation(
     "Generating new question set for {Title} ({Difficulty}, {QuestionCount} questions)",
         snapshot.Title,
            difficulty,
 questionCount);

var newSet = await GenerateNewQuestionSetAsync(
          snapshot,
         difficulty,
 questionCount,
      languageCode,
      factsHash,
  userId,
 cancellationToken);

        return newSet;
    }

    private async Task<QuestionSet> GenerateNewQuestionSetAsync(
TitleFactsSnapshot snapshot,
        QuizDifficulty difficulty,
        int questionCount,
        string languageCode,
        string factsHash,
        Guid userId,
    CancellationToken cancellationToken)
    {
        // Generate questions using AI
        var generatedQuestions = await _questionGenerator.GenerateQuestionsAsync(
            new QuestionGenerationRequest
     {
        Snapshot = snapshot,
       Difficulty = difficulty,
                QuestionCount = questionCount,
         LanguageCode = languageCode
            },
    cancellationToken);

     // Create question set entity
 var questionSet = new QuestionSet
    {
            Id = Guid.NewGuid(),
            TitleType = snapshot.TitleType,
         TmdbId = snapshot.TmdbId,
       Difficulty = difficulty,
      LanguageCode = languageCode,
    FactsHash = factsHash,
     PromptVersion = CurrentPromptVersion,
            CreatedAt = DateTime.UtcNow,
 UsesCount = 1,
IsActive = true
        };

        // Create question entities
     var questions = generatedQuestions.Select((gq, index) => new QuestionSetQuestion
        {
            Id = Guid.NewGuid(),
          SetId = questionSet.Id,
    Ordinal = index + 1,
   Text = gq.Text,
      Options = gq.Options,
  CorrectIndex = gq.CorrectIndex,
    ImageUrl = gq.ImageUrl,
  SourceReference = gq.SourceReference,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        // Save to database
      await _questionSetRepository.CreateWithQuestionsAsync(questionSet, questions, cancellationToken);

        // Mark as played by this user
        await _questionSetRepository.MarkAsPlayedAsync(questionSet.Id, userId, cancellationToken);

  _logger.LogInformation(
   "Created new question set {SetId} with {Count} questions for {Title}",
     questionSet.Id,
          questions.Count,
         snapshot.Title);

        // Reload with questions
  return (await _questionSetRepository.GetWithQuestionsAsync(questionSet.Id, cancellationToken))!;
    }

    private string CalculateFactsHash(TitleFactsSnapshot snapshot)
    {
        // Create a stable representation of the snapshot facts
        var factsObject = new
        {
 snapshot.TmdbId,
            snapshot.TitleType,
   snapshot.Title,
      snapshot.Year,
  snapshot.Overview,
       Genres = string.Join("|", snapshot.Genres.OrderBy(g => g)),
   Cast = string.Join("|", snapshot.CastTop.Take(10).Select(c => $"{c.Name}:{c.Character}").OrderBy(x => x)),
      CreatedBy = string.Join("|", snapshot.CreatedBy.OrderBy(c => c)),
 snapshot.SeasonsCount,
            ProductionCompanies = string.Join("|", snapshot.ProductionCompanies.OrderBy(p => p)),
  OriginCountry = string.Join("|", snapshot.OriginCountry.OrderBy(o => o)),
 Keywords = string.Join("|", snapshot.Keywords.Take(10).OrderBy(k => k)),
      Collection = snapshot.BelongsToCollection?.Name ?? string.Empty
};

        var json = JsonSerializer.Serialize(factsObject);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = SHA256.HashData(bytes);
        
        return Convert.ToHexString(hash);
    }
}
