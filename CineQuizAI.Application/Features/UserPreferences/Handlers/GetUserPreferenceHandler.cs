using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.UserPreferences.DTOs;
using CineQuizAI.Application.Features.UserPreferences.Queries;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.UserPreferences.Handlers;

/// <summary>
/// Handler for getting user preferences
/// </summary>
public sealed class GetUserPreferenceHandler
{
    private readonly IUserPreferenceRepository _repository;

    public GetUserPreferenceHandler(IUserPreferenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserPreferenceDto> HandleAsync(
        GetUserPreferenceQuery query,
    CancellationToken cancellationToken = default)
    {
    var preference = await _repository.GetByUserIdAsync(query.UserId, cancellationToken);

  // Return default preferences if none exist
        if (preference is null)
    {
      return new UserPreferenceDto
          {
        UserId = query.UserId,
   DefaultCategory = QuizCategory.Movie,
     DefaultDifficulty = QuizDifficulty.Medium,
       LanguageCode = "sv-SE",
              CreatedAt = DateTime.UtcNow,
       UpdatedAt = DateTime.UtcNow
            };
        }

        return new UserPreferenceDto
        {
    UserId = preference.UserId,
            DefaultCategory = preference.DefaultCategory,
          DefaultDifficulty = preference.DefaultDifficulty,
        LanguageCode = preference.LanguageCode,
            CreatedAt = preference.CreatedAt,
            UpdatedAt = preference.UpdatedAt
        };
    }
}
