using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A reusable set of questions for a specific title, difficulty, and language
/// </summary>
public class QuestionSet
{
    public Guid Id { get; set; }
    public TitleType TitleType { get; set; }
    public long TmdbId { get; set; }
    public QuizDifficulty Difficulty { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string FactsHash { get; set; } = string.Empty; // SHA-256 of snapshot facts
    public int PromptVersion { get; set; } // Incremented when AI prompt changes
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int UsesCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public ICollection<QuestionSetQuestion> Questions { get; set; } = new List<QuestionSetQuestion>();
    public ICollection<UserQuestionSetPlay> Plays { get; set; } = new List<UserQuestionSetPlay>();
}
