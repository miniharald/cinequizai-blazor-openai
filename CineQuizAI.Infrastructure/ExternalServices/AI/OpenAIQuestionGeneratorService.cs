using System.ClientModel;
using System.Text.Json;
using CineQuizAI.Application.Abstractions.AI;
using CineQuizAI.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace CineQuizAI.Infrastructure.ExternalServices.AI;

/// <summary>
/// OpenAI-based question generator service
/// </summary>
public sealed class OpenAIQuestionGeneratorService : IQuestionGeneratorService
{
    private readonly ChatClient _chatClient;
    private readonly OpenAIConfiguration _config;
    private readonly ILogger<OpenAIQuestionGeneratorService> _logger;

    public OpenAIQuestionGeneratorService(
     IOptions<OpenAIConfiguration> config,
    ILogger<OpenAIQuestionGeneratorService> logger)
 {
    _config = config.Value;
   _logger = logger;
  
    var apiKey = new ApiKeyCredential(_config.ApiKey);
        _chatClient = new ChatClient(_config.Model, apiKey);
    }

    public async Task<List<GeneratedQuestion>> GenerateQuestionsAsync(
    QuestionGenerationRequest request,
  CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine($"?? OpenAI: Starting question generation...");
     var systemPrompt = BuildSystemPrompt(request);
         var userPrompt = BuildUserPrompt(request);

  _logger.LogInformation(
 "Generating {Count} questions for {Title} ({TitleType}, {Difficulty}, {Language})",
  request.QuestionCount,
   request.Snapshot.Title,
            request.Snapshot.TitleType,
                request.Difficulty,
  request.LanguageCode);

            Console.WriteLine($"?? OpenAI: Building messages...");
            var messages = new List<ChatMessage>
    {
            new SystemChatMessage(systemPrompt),
         new UserChatMessage(userPrompt)
     };

            var options = new ChatCompletionOptions
            {
 MaxOutputTokenCount = _config.MaxTokens,
    Temperature = (float)_config.Temperature
      };

    Console.WriteLine($"?? OpenAI: Calling ChatClient.CompleteChatAsync...");
            Console.WriteLine($"   Model: {_config.Model}");
 Console.WriteLine($"   MaxTokens: {_config.MaxTokens}");
            Console.WriteLine($"   Temperature: {_config.Temperature}");
            
     var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);

 Console.WriteLine($"?? OpenAI: Received response!");
            var content = response.Value.Content[0].Text;
            Console.WriteLine($"?? OpenAI: Response length: {content.Length} characters");

        Console.WriteLine($"?? OpenAI: Parsing questions...");
            var questions = ParseQuestionsFromResponse(content);

      _logger.LogInformation(
  "Successfully generated {Count} questions for {Title}",
          questions.Count,
       request.Snapshot.Title);

            Console.WriteLine($"? OpenAI: Successfully generated {questions.Count} questions!");
          return questions;
        }
        catch (Exception ex)
        {
         Console.WriteLine($"? OpenAI: Exception occurred!");
            Console.WriteLine($"   Type: {ex.GetType().Name}");
   Console.WriteLine($"   Message: {ex.Message}");
            Console.WriteLine($"   Stack: {ex.StackTrace}");
 
          _logger.LogError(ex, "Failed to generate questions for {Title}", request.Snapshot.Title);
        throw;
        }
    }

    private string BuildSystemPrompt(QuestionGenerationRequest request)
    {
        var language = request.LanguageCode.StartsWith("sv") ? "svenska" : "English";
        var titleType = request.Snapshot.TitleType == TitleType.Movie ? "film" : "TV-serie";

    return $@"Du är en expert på att skapa quiz-frågor om filmer och TV-serier.

Regler:
1. Generera frågor ENDAST baserat på den fakta som ges i användarens meddelande.
2. Använd INTE extern kunskap eller information utanför det som ges.
3. Skapa rättvisa flervalsfrågor med EXAKT 4 svarsalternativ.
4. Alla frågor ska vara på {language}.
5. Frågor ska vara lämpliga för svårighetsgrad: {request.Difficulty}.
6. Fokusera på: skådespelare, regissörer, genre, årtal, handling, production companies.
7. Undvik spoilers och extremt detaljerade frågor om {titleType} inte är väldigt känd.

Svårighetsgrad-guide:
- Easy: Allmän kännedom (huvudskådespelare, årtal, genre)
- Medium: Mer detaljerat (birollen, skapare, produktionsbolag)
- Hard: Nischad kunskap (mindre kända skådespelare, specifika detaljer)

Svara ENDAST med en JSON-array med följande format:
[
  {{
    ""text"": ""Frågetexten här?"",
 ""options"": [""Alternativ 1"", ""Alternativ 2"", ""Alternativ 3"", ""Alternativ 4""],
    ""correctIndex"": 0,
 ""sourceReference"": ""kort referens till TMDb-fältet""
  }}
]

VIKTIGT: 
- correctIndex är 0-baserat (0 = första alternativet)
- Exakt 4 alternativ per fråga
- Endast giltig JSON, ingen extra text";
    }

  private string BuildUserPrompt(QuestionGenerationRequest request)
    {
    var snapshot = request.Snapshot;
 var titleType = snapshot.TitleType == TitleType.Movie ? "Film" : "TV-serie";

 var facts = new
{
  TitleType = titleType,
  Title = snapshot.Title,
  Year = snapshot.Year,
  Overview = snapshot.Overview,
  Genres = snapshot.Genres,
  Cast = snapshot.CastTop.Take(10).Select(c => new { c.Name, c.Character }).ToList(),
  CreatedBy = snapshot.CreatedBy,
  SeasonsCount = snapshot.SeasonsCount,
  ProductionCompanies = snapshot.ProductionCompanies,
    OriginCountry = snapshot.OriginCountry,
     OriginalLanguage = snapshot.OriginalLanguage,
     Keywords = snapshot.Keywords.Take(10).ToList(),
 Collection = snapshot.BelongsToCollection?.Name
        };

    var factsJson = JsonSerializer.Serialize(facts, new JsonSerializerOptions
    {
    WriteIndented = true,
PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

  return $@"Skapa {request.QuestionCount} quiz-frågor baserat ENDAST på denna fakta:

{factsJson}

Generera {request.QuestionCount} frågor på {(request.LanguageCode.StartsWith("sv") ? "svenska" : "engelska")} med svårighetsgrad {request.Difficulty}.";
 }

    private List<GeneratedQuestion> ParseQuestionsFromResponse(string jsonResponse)
    {
        try
        {
  // Remove markdown code blocks if present
   var cleanJson = jsonResponse.Trim();
  if (cleanJson.StartsWith("```json"))
 {
         cleanJson = cleanJson.Substring(7);
      }
     if (cleanJson.StartsWith("```"))
        {
     cleanJson = cleanJson.Substring(3);
  }
        if (cleanJson.EndsWith("```"))
   {
      cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
  }
         cleanJson = cleanJson.Trim();

     var questions = JsonSerializer.Deserialize<List<GeneratedQuestion>>(cleanJson, new JsonSerializerOptions
{
 PropertyNameCaseInsensitive = true
});

 if (questions == null || questions.Count == 0)
      {
    throw new InvalidOperationException("No questions were generated");
 }

     // Validate each question
   foreach (var question in questions)
  {
   if (question.Options.Count != 4)
  {
 throw new InvalidOperationException($"Question must have exactly 4 options, got {question.Options.Count}");
}

   if (question.CorrectIndex < 0 || question.CorrectIndex > 3)
 {
    throw new InvalidOperationException($"CorrectIndex must be between 0 and 3, got {question.CorrectIndex}");
 }
        }

   return questions;
}
      catch (JsonException ex)
    {
     _logger.LogError(ex, "Failed to parse OpenAI response as JSON. Response: {Response}", jsonResponse);
  throw new InvalidOperationException("Failed to parse OpenAI response", ex);
        }
    }
}
