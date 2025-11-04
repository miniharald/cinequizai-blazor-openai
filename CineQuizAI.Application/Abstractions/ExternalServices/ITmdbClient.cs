using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;

namespace CineQuizAI.Application.Abstractions.ExternalServices;

/// <summary>
/// Interface for TMDb API client
/// </summary>
public interface ITmdbClient
{
    /// <summary>
    /// Fetches and creates a TitleFactsSnapshot for a movie
    /// </summary>
    /// <param name="tmdbId">TMDb movie ID</param>
    /// <param name="languageCode">Language code (e.g., "sv-SE", "en-US")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>TitleFactsSnapshot for the movie</returns>
    Task<TitleFactsSnapshot> GetMovieSnapshotAsync(long tmdbId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches and creates a TitleFactsSnapshot for a TV series
    /// </summary>
    /// <param name="tmdbId">TMDb TV series ID</param>
    /// <param name="languageCode">Language code (e.g., "sv-SE", "en-US")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>TitleFactsSnapshot for the TV series</returns>
    Task<TitleFactsSnapshot> GetTvSeriesSnapshotAsync(long tmdbId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a snapshot based on title type
    /// </summary>
    /// <param name="tmdbId">TMDb ID</param>
    /// <param name="titleType">Type of title (Movie or Tv)</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>TitleFactsSnapshot</returns>
    Task<TitleFactsSnapshot> GetTitleSnapshotAsync(long tmdbId, TitleType titleType, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for movies by title
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of search results with basic info</returns>
    Task<List<TmdbSearchResult>> SearchMoviesAsync(string query, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for TV series by title
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of search results with basic info</returns>
    Task<List<TmdbSearchResult>> SearchTvSeriesAsync(string query, string languageCode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Simplified search result from TMDb
/// </summary>
public class TmdbSearchResult
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterPath { get; set; }
    public int? Year { get; set; }
  public string? Overview { get; set; }
}
