using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// Membership in a private leaderboard
/// </summary>
public class PrivateLeaderboardMember
{
    public Guid LeaderboardId { get; set; }
    public Guid UserId { get; set; }
    public LeaderboardRole Role { get; set; } = LeaderboardRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public PrivateLeaderboard Leaderboard { get; set; } = null!;
}
