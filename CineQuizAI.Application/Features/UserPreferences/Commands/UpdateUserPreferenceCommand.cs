using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.UserPreferences.Commands;

/// <summary>
/// Command to update user preferences
/// </summary>
public sealed record UpdateUserPreferenceCommand
{
    public Guid UserId { get; init; }
    public QuizCategory DefaultCategory { get; init; }
    public QuizDifficulty DefaultDifficulty { get; init; }
    public string LanguageCode { get; init; } = "sv-SE";
}
