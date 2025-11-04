using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A user report about a problematic quiz question
/// </summary>
public class QuestionReport
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
public Guid ReporterUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ReportStatus Status { get; set; } = ReportStatus.Open;
 
    // Navigation property
    public QuizQuestion Question { get; set; } = null!;
}
