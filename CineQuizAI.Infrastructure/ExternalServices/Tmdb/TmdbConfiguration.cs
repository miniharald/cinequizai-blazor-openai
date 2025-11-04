namespace CineQuizAI.Infrastructure.ExternalServices.Tmdb;

/// <summary>
/// Configuration for TMDb API
/// </summary>
public class TmdbConfiguration
{
    public const string SectionName = "Tmdb";

    /// <summary>
    /// TMDb API Key (Bearer token for v4 API)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for TMDb API (default: https://api.themoviedb.org/3)
  /// </summary>
    public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";

    /// <summary>
    /// Base URL for TMDb images (default: https://image.tmdb.org/t/p/)
    /// </summary>
    public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/";

    /// <summary>
    /// Default poster size (default: w500)
    /// </summary>
    public string DefaultPosterSize { get; set; } = "w500";

    /// <summary>
    /// Request timeout in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

/// <summary>
    /// Maximum number of retry attempts (default: 3)
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
}
