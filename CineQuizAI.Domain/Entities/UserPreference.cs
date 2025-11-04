using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// User's default quiz preferences
/// </summary>
public class UserPreference
{
    public Guid UserId { get; set; }
    public QuizCategory DefaultCategory { get; set; } = QuizCategory.Movie;
    public QuizDifficulty DefaultDifficulty { get; set; } = QuizDifficulty.Medium;
    public string LanguageCode { get; set; } = "sv-SE";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
