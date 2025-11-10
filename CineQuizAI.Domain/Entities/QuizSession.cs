using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A quiz session played by a user
/// </summary>
public class QuizSession
{
    public Guid Id { get; set; }
public Guid? UserId { get; set; } // Nullable to preserve history if user is deleted
    public QuizVisibility Visibility { get; set; } = QuizVisibility.Public;
    public QuizCategory Category { get; set; }
    public QuizDifficulty Difficulty { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public long TmdbId { get; set; }
    public TitleType TitleType { get; set; }
    public Guid SnapshotId { get; set; }
    public int QuestionCount { get; set; }
 public int Score { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    
    // Navigation properties
    public TitleFactsSnapshot Snapshot { get; set; } = null!;
    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}
