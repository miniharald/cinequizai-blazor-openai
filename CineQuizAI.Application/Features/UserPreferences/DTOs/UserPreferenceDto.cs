using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Features.UserPreferences.DTOs;

/// <summary>
/// DTO for user preferences
/// </summary>
public sealed class UserPreferenceDto
{
    public Guid UserId { get; set; }
    public QuizCategory DefaultCategory { get; set; }
    public QuizDifficulty DefaultDifficulty { get; set; }
    public string LanguageCode { get; set; } = "sv-SE";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
