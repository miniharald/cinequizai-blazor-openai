using System.Net.Http.Json;
using System.Text.Json;
using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using CineQuizAI.Domain.ValueObjects;
using CineQuizAI.Infrastructure.ExternalServices.Tmdb.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CineQuizAI.Infrastructure.ExternalServices.Tmdb;

/// <summary>
/// Implementation of TMDb API client
/// </summary>
public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TmdbClient> _logger;
    private readonly TmdbConfiguration _config;
  private readonly JsonSerializerOptions _jsonOptions;

    public TmdbClient(
        HttpClient httpClient,
        ILogger<TmdbClient> logger,
        IOptions<TmdbConfiguration> config)
    {
     _httpClient = httpClient;
 _logger = logger;
     _config = config.Value;

        // Configure JSON serialization
        _jsonOptions = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
     DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
  };

        // Configure HttpClient
 // Ensure BaseUrl ends with / for proper relative URL resolution
   var baseUrl = _config.BaseUrl.TrimEnd('/') + "/";
        _httpClient.BaseAddress = new Uri(baseUrl);
      _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
    }

    public async Task<TitleFactsSnapshot> GetMovieSnapshotAsync(
        long tmdbId,
        string languageCode,
   CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching movie snapshot for TMDb ID: {TmdbId}, Language: {Language}", tmdbId, languageCode);

 var url = $"movie/{tmdbId}?language={languageCode}&append_to_response=credits,keywords";
        
        var movieDetails = await GetAsync<TmdbMovieDetails>(url, cancellationToken);
        
  if (movieDetails == null)
        {
   throw new InvalidOperationException($"Failed to fetch movie details for TMDb ID: {tmdbId}");
        }

    return MapMovieToSnapshot(movieDetails, languageCode);
    }

    public async Task<TitleFactsSnapshot> GetTvSeriesSnapshotAsync(
long tmdbId,
   string languageCode,
        CancellationToken cancellationToken = default)
    {
_logger.LogInformation("Fetching TV series snapshot for TMDb ID: {TmdbId}, Language: {Language}", tmdbId, languageCode);

        var url = $"tv/{tmdbId}?language={languageCode}&append_to_response=credits,keywords";
        
        var tvDetails = await GetAsync<TmdbTvDetails>(url, cancellationToken);
        
        if (tvDetails == null)
   {
            throw new InvalidOperationException($"Failed to fetch TV series details for TMDb ID: {tmdbId}");
        }

        return MapTvSeriesToSnapshot(tvDetails, languageCode);
    }

    public Task<TitleFactsSnapshot> GetTitleSnapshotAsync(
        long tmdbId,
        TitleType titleType,
      string languageCode,
        CancellationToken cancellationToken = default)
    {
        return titleType switch
     {
        TitleType.Movie => GetMovieSnapshotAsync(tmdbId, languageCode, cancellationToken),
          TitleType.Tv => GetTvSeriesSnapshotAsync(tmdbId, languageCode, cancellationToken),
         _ => throw new ArgumentException($"Unknown title type: {titleType}", nameof(titleType))
        };
    }

    public async Task<List<TmdbSearchResult>> SearchMoviesAsync(
        string query,
  string languageCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("?? Searching movies: Query='{Query}', Language='{Language}'", query, languageCode);

 var url = $"search/movie?query={Uri.EscapeDataString(query)}&language={languageCode}";
     
        _logger.LogInformation("?? TMDb API URL: {BaseUrl}{Url}", _httpClient.BaseAddress, url);

   var response = await GetAsync<TmdbSearchResponse>(url, cancellationToken);
      
        if (response == null)
     {
  _logger.LogWarning("?? TMDb API returned null response for movie search");
    return new List<TmdbSearchResult>();
  }

  _logger.LogInformation("? TMDb API returned {Count} movie results", response.Results.Count);

        return response.Results.Select(MapSearchItemToResult).ToList();
  }

    public async Task<List<TmdbSearchResult>> SearchTvSeriesAsync(
    string query,
   string languageCode,
      CancellationToken cancellationToken = default)
  {
        _logger.LogInformation("?? Searching TV series: Query='{Query}', Language='{Language}'", query, languageCode);

    var url = $"search/tv?query={Uri.EscapeDataString(query)}&language={languageCode}";
        
        _logger.LogInformation("?? TMDb API URL: {BaseUrl}{Url}", _httpClient.BaseAddress, url);

        var response = await GetAsync<TmdbSearchResponse>(url, cancellationToken);
  
    if (response == null)
      {
  _logger.LogWarning("?? TMDb API returned null response for TV search");
   return new List<TmdbSearchResult>();
    }

  _logger.LogInformation("? TMDb API returned {Count} TV results", response.Results.Count);

   return response.Results.Select(MapSearchItemToResult).ToList();
    }

    #region Private Methods

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken) where T : class
    {
        try
     {
     var response = await _httpClient.GetAsync(url, cancellationToken);
          
   if (!response.IsSuccessStatusCode)
            {
     var content = await response.Content.ReadAsStringAsync(cancellationToken);
     _logger.LogWarning(
    "TMDb API request failed. Status: {StatusCode}, URL: {Url}, Response: {Response}",
        response.StatusCode,
         url,
      content);
    return null;
            }

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
      {
            _logger.LogError(ex, "HTTP request exception when calling TMDb API: {Url}", url);
  throw;
        }
        catch (TaskCanceledException ex)
        {
         _logger.LogError(ex, "Request timeout when calling TMDb API: {Url}", url);
  throw;
      }
    }

    private TitleFactsSnapshot MapMovieToSnapshot(TmdbMovieDetails movie, string languageCode)
    {
        var year = ExtractYear(movie.ReleaseDate);

   return new TitleFactsSnapshot
    {
      Id = Guid.NewGuid(),
  TmdbId = movie.Id,
   TitleType = TitleType.Movie,
            LanguageCode = languageCode,
       Title = movie.Title,
      Overview = movie.Overview,
            Year = year,
            Genres = movie.Genres.Select(g => g.Name).ToList(),
      CastTop = ExtractTopCast(movie.Credits),
            CreatedBy = new List<string>(), // Not applicable for movies
            SeasonsCount = null, // Not applicable for movies
            ProductionCompanies = movie.ProductionCompanies
     .Where(pc => IsWellKnownCompany(pc.Name))
      .Select(pc => pc.Name)
  .ToList(),
       OriginCountry = movie.ProductionCountries.Select(pc => pc.Iso31661).ToList(),
          OriginalLanguage = movie.OriginalLanguage,
      PosterUrl = BuildPosterUrl(movie.PosterPath),
         Keywords = ExtractKeywords(movie.Keywords),
            BelongsToCollection = movie.BelongsToCollection != null
              ? new Collection
 {
           Id = movie.BelongsToCollection.Id,
        Name = movie.BelongsToCollection.Name
       }
   : null,
            FetchedAt = DateTime.UtcNow
        };
 }

    private TitleFactsSnapshot MapTvSeriesToSnapshot(TmdbTvDetails tvSeries, string languageCode)
    {
   var year = ExtractYear(tvSeries.FirstAirDate);

        return new TitleFactsSnapshot
   {
  Id = Guid.NewGuid(),
            TmdbId = tvSeries.Id,
    TitleType = TitleType.Tv,
  LanguageCode = languageCode,
          Title = tvSeries.Name,
            Overview = tvSeries.Overview,
   Year = year,
            Genres = tvSeries.Genres.Select(g => g.Name).ToList(),
            CastTop = ExtractTopCast(tvSeries.Credits),
    CreatedBy = tvSeries.CreatedBy.Select(c => c.Name).ToList(),
SeasonsCount = tvSeries.NumberOfSeasons,
 ProductionCompanies = tvSeries.ProductionCompanies
  .Where(pc => IsWellKnownCompany(pc.Name))
        .Select(pc => pc.Name)
             .ToList(),
  OriginCountry = tvSeries.OriginCountry,
            OriginalLanguage = tvSeries.OriginalLanguage,
    PosterUrl = BuildPosterUrl(tvSeries.PosterPath),
         Keywords = ExtractKeywords(tvSeries.Keywords),
          BelongsToCollection = null, // Not applicable for TV series
    FetchedAt = DateTime.UtcNow
        };
    }

    private List<CastMember> ExtractTopCast(TmdbCredits? credits, int maxCount = 10)
    {
        if (credits?.Cast == null || credits.Cast.Count == 0)
        {
         return new List<CastMember>();
        }

        return credits.Cast
      .OrderBy(c => c.Order)
            .Take(maxCount)
   .Select(c => new CastMember
        {
         Name = c.Name,
           Character = c.Character
  })
          .ToList();
    }

    private List<string> ExtractKeywords(TmdbKeywordsContainer? keywordsContainer)
    {
        if (keywordsContainer == null)
        {
            return new List<string>();
        }

        // Keywords can be in either "keywords" or "results" depending on endpoint
        var keywords = keywordsContainer.Keywords ?? keywordsContainer.Results ?? new List<TmdbKeyword>();

        return keywords
            .Select(k => k.Name)
       .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();
  }

    private string? BuildPosterUrl(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
    return null;
    }

        return $"{_config.ImageBaseUrl}{_config.DefaultPosterSize}{posterPath}";
    }

    private int ExtractYear(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
     return 0;
      }

        if (DateTime.TryParse(dateString, out var date))
        {
         return date.Year;
        }

        // Try to extract just the year if date parsing fails
        if (dateString.Length >= 4 && int.TryParse(dateString.Substring(0, 4), out var year))
        {
    return year;
}

        return 0;
    }

    private bool IsWellKnownCompany(string companyName)
    {
        // List of well-known production companies/studios
        var wellKnownCompanies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
   {
          "Netflix", "Disney", "Disney+", "Marvel Studios", "Lucasfilm",
"Warner Bros", "HBO", "Amazon Studios", "Prime Video",
     "Universal Pictures", "Paramount", "Sony Pictures", "20th Century",
 "Pixar", "DreamWorks", "A24", "Lionsgate", "MGM", "Apple TV+",
     "Hulu", "Showtime", "AMC", "FX", "BBC", "ITV", "Channel 4"
        };

        return wellKnownCompanies.Any(known => companyName.Contains(known, StringComparison.OrdinalIgnoreCase));
    }

    private TmdbSearchResult MapSearchItemToResult(TmdbSearchItem item)
    {
    var title = item.Title ?? item.Name ?? string.Empty;
        var dateString = item.ReleaseDate ?? item.FirstAirDate;
        var year = ExtractYear(dateString);

     return new TmdbSearchResult
  {
     Id = item.Id,
        Title = title,
       PosterPath = BuildPosterUrl(item.PosterPath),
            Year = year > 0 ? year : null,
          Overview = item.Overview
        };
    }

    #endregion
}
