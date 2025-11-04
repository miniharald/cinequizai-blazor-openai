using CineQuizAI.Application.Abstractions.Persistence;
using CineQuizAI.Application.Features.UserPreferences.Commands;
using CineQuizAI.Application.Features.UserPreferences.DTOs;
using CineQuizAI.Domain.Entities;

namespace CineQuizAI.Application.Features.UserPreferences.Handlers;

/// <summary>
/// Handler for updating user preferences
/// </summary>
public sealed class UpdateUserPreferenceHandler
{
    private readonly IUserPreferenceRepository _repository;

    public UpdateUserPreferenceHandler(IUserPreferenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserPreferenceDto> HandleAsync(
        UpdateUserPreferenceCommand command,
        CancellationToken cancellationToken = default)
    {
        var preference = await _repository.GetByUserIdAsync(command.UserId, cancellationToken);

        if (preference is null)
        {
            // Create new preference
            preference = new UserPreference
            {
                UserId = command.UserId,
                DefaultCategory = command.DefaultCategory,
                DefaultDifficulty = command.DefaultDifficulty,
                LanguageCode = command.LanguageCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            preference = await _repository.CreateAsync(preference, cancellationToken);
        }
        else
        {
            // Update existing preference
            preference.DefaultCategory = command.DefaultCategory;
            preference.DefaultDifficulty = command.DefaultDifficulty;
            preference.LanguageCode = command.LanguageCode;
            preference.UpdatedAt = DateTime.UtcNow;

            preference = await _repository.UpdateAsync(preference, cancellationToken);
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
