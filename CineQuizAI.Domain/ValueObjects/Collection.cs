namespace CineQuizAI.Domain.ValueObjects;

/// <summary>
/// Represents a collection/franchise that a movie belongs to
/// </summary>
public sealed record Collection
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
