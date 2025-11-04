namespace CineQuizAI.Domain.ValueObjects;

/// <summary>
/// Represents a cast member in TMDb data
/// </summary>
public sealed record CastMember
{
    public string Name { get; init; } = string.Empty;
    public string Character { get; init; } = string.Empty;
    public int Order { get; init; }
}
