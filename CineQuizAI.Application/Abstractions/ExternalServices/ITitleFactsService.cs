using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Abstractions.ExternalServices;

/// <summary>
/// Service for managing TitleFactsSnapshot caching and retrieval
/// </summary>
public interface ITitleFactsService
{
    /// <summary>
    /// Gets or creates a TitleFactsSnapshot for a given title
    /// Uses cached version if available and not expired
    /// </summary>
    /// <param name="tmdbId">TMDb ID</param>
    /// <param name="titleType">Type of title (Movie or Tv)</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>TitleFactsSnapshot</returns>
    Task<TitleFactsSnapshot> GetOrCreateSnapshotAsync(
     long tmdbId,
        TitleType titleType,
        string languageCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces a refresh of the snapshot from TMDb
    /// </summary>
    /// <param name="tmdbId">TMDb ID</param>
    /// <param name="titleType">Type of title</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated TitleFactsSnapshot</returns>
    Task<TitleFactsSnapshot> RefreshSnapshotAsync(
        long tmdbId,
        TitleType titleType,
        string languageCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a snapshot needs to be refreshed based on TTL
    /// </summary>
  /// <param name="snapshot">The snapshot to check</param>
    /// <returns>True if snapshot should be refreshed</returns>
    bool ShouldRefreshSnapshot(TitleFactsSnapshot snapshot);
}
