namespace CineQuizAI.Domain.Entities;

/// <summary>
/// Records that a user has played a specific question set
/// </summary>
public class UserQuestionSetPlay
{
    public Guid UserId { get; set; }
    public Guid SetId { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public QuestionSet Set { get; set; } = null!;
}
