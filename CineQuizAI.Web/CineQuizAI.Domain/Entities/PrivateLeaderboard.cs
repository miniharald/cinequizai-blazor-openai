using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// Private leaderboard created by a user
/// </summary>
public class PrivateLeaderboard
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public LeaderboardVisibility Visibility { get; set; } = LeaderboardVisibility.InviteOnly;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
 public ICollection<PrivateLeaderboardMember> Members { get; set; } = new List<PrivateLeaderboardMember>();
}
