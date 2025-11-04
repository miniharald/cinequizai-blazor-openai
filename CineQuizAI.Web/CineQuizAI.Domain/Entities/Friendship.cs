using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// Friendship relationship between two users
/// </summary>
public class Friendship
{
    public Guid Id { get; set; }
 public Guid RequesterUserId { get; set; }
  public Guid AddresseeUserId { get; set; }
  public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
