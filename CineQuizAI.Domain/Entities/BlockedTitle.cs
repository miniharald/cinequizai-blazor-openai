using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// A title that has been blocked from appearing in quizzes
/// </summary>
public class BlockedTitle
{
    public Guid Id { get; set; }
    public long TmdbId { get; set; }
    public TitleType TitleType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
