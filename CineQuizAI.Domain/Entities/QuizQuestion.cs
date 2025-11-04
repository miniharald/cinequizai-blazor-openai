namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A question in a quiz session
/// </summary>
public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public int Ordinal { get; set; }
    public string Text { get; set; } = string.Empty;
    
 // JSONB field - will store exactly 4 options
    public List<string> Options { get; set; } = new();

    public int CorrectIndex { get; set; }
    public string? ImageUrl { get; set; }
    public string? SourceReference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public QuizSession Session { get; set; } = null!;
    public QuestionAnswer? Answer { get; set; }
}
