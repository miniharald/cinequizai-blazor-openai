namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A question belonging to a reusable question set
/// </summary>
public class QuestionSetQuestion
{
    public Guid Id { get; set; }
    public Guid SetId { get; set; }
    public int Ordinal { get; set; }
    public string Text { get; set; } = string.Empty;
    
    // JSONB field - will store exactly 4 options
    public List<string> Options { get; set; } = new();
    
    public int CorrectIndex { get; set; }
    public string? ImageUrl { get; set; }
    public string? SourceReference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
public QuestionSet Set { get; set; } = null!;
}
