namespace CineQuizAI.Domain.Entities;

/// <summary>
/// User's answer to a quiz question
/// </summary>
public class QuestionAnswer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public int SelectedIndex { get; set; }
    public bool IsCorrect { get; set; }
    public int TimeSpentMs { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
  
    // Navigation property
    public QuizQuestion Question { get; set; } = null!;
}
