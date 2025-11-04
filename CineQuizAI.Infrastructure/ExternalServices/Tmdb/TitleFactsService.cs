using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using CineQuizAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CineQuizAI.Infrastructure.ExternalServices.Tmdb;

/// <summary>
/// Implementation of TitleFactsService with database and memory caching
/// </summary>
public class TitleFactsService : ITitleFactsService
{
    private readonly ITmdbClient _tmdbClient;
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TitleFactsService> _logger;

    // Cache TTL configuration
    private static readonly TimeSpan MovieTtl = TimeSpan.FromDays(7);
    private static readonly TimeSpan TvSeriesTtl = TimeSpan.FromDays(3);
private static readonly TimeSpan MemoryCacheTtl = TimeSpan.FromHours(1);

    public TitleFactsService(
        ITmdbClient tmdbClient,
        AppDbContext dbContext,
        IMemoryCache memoryCache,
        ILogger<TitleFactsService> logger)
    {
        _tmdbClient = tmdbClient;
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<TitleFactsSnapshot> GetOrCreateSnapshotAsync(
        long tmdbId,
        TitleType titleType,
      string languageCode,
        CancellationToken cancellationToken = default)
    {
   var cacheKey = BuildCacheKey(tmdbId, titleType, languageCode);

        // 1. Try memory cache first
        if (_memoryCache.TryGetValue<TitleFactsSnapshot>(cacheKey, out var cachedSnapshot) && cachedSnapshot != null)
  {
            _logger.LogDebug("Snapshot found in memory cache: {CacheKey}", cacheKey);
            
   if (!ShouldRefreshSnapshot(cachedSnapshot))
  {
                return cachedSnapshot;
  }
        }

        // 2. Try database
        var dbSnapshot = await _dbContext.TitleFactsSnapshots
            .FirstOrDefaultAsync(
                s => s.TmdbId == tmdbId && s.TitleType == titleType && s.LanguageCode == languageCode,
    cancellationToken);

        if (dbSnapshot != null)
        {
   _logger.LogDebug("Snapshot found in database: {TmdbId}, {TitleType}, {Language}", tmdbId, titleType, languageCode);

     if (!ShouldRefreshSnapshot(dbSnapshot))
     {
       // Cache in memory and return
         _memoryCache.Set(cacheKey, dbSnapshot, MemoryCacheTtl);
       return dbSnapshot;
    }

      // Snapshot exists but is stale, refresh it
         return await RefreshExistingSnapshotAsync(dbSnapshot, cancellationToken);
        }

        // 3. Create new snapshot from TMDb
        _logger.LogInformation("Creating new snapshot from TMDb: {TmdbId}, {TitleType}, {Language}", tmdbId, titleType, languageCode);
        return await CreateNewSnapshotAsync(tmdbId, titleType, languageCode, cancellationToken);
}

    public async Task<TitleFactsSnapshot> RefreshSnapshotAsync(
     long tmdbId,
        TitleType titleType,
        string languageCode,
   CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Force refreshing snapshot: {TmdbId}, {TitleType}, {Language}", tmdbId, titleType, languageCode);

        var existingSnapshot = await _dbContext.TitleFactsSnapshots
            .FirstOrDefaultAsync(
            s => s.TmdbId == tmdbId && s.TitleType == titleType && s.LanguageCode == languageCode,
       cancellationToken);

        if (existingSnapshot != null)
        {
            return await RefreshExistingSnapshotAsync(existingSnapshot, cancellationToken);
        }

        return await CreateNewSnapshotAsync(tmdbId, titleType, languageCode, cancellationToken);
    }

    public bool ShouldRefreshSnapshot(TitleFactsSnapshot snapshot)
    {
        var ttl = snapshot.TitleType == TitleType.Movie ? MovieTtl : TvSeriesTtl;
var age = DateTime.UtcNow - snapshot.FetchedAt;

        return age > ttl;
    }

 #region Private Methods

    private async Task<TitleFactsSnapshot> CreateNewSnapshotAsync(
        long tmdbId,
        TitleType titleType,
        string languageCode,
    CancellationToken cancellationToken)
    {
        // Fetch from TMDb
var snapshot = await _tmdbClient.GetTitleSnapshotAsync(tmdbId, titleType, languageCode, cancellationToken);

        // Save to database
 _dbContext.TitleFactsSnapshots.Add(snapshot);
        await _dbContext.SaveChangesAsync(cancellationToken);

  // Cache in memory
        var cacheKey = BuildCacheKey(tmdbId, titleType, languageCode);
        _memoryCache.Set(cacheKey, snapshot, MemoryCacheTtl);

     _logger.LogInformation("New snapshot created and cached: {TmdbId}, {TitleType}, {Language}", tmdbId, titleType, languageCode);

        return snapshot;
 }

    private async Task<TitleFactsSnapshot> RefreshExistingSnapshotAsync(
    TitleFactsSnapshot existingSnapshot,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refreshing existing snapshot: {Id}", existingSnapshot.Id);

        // Fetch fresh data from TMDb
        var freshSnapshot = await _tmdbClient.GetTitleSnapshotAsync(
            existingSnapshot.TmdbId,
    existingSnapshot.TitleType,
   existingSnapshot.LanguageCode,
   cancellationToken);

        // Update existing snapshot with fresh data
        existingSnapshot.Title = freshSnapshot.Title;
   existingSnapshot.Overview = freshSnapshot.Overview;
        existingSnapshot.Year = freshSnapshot.Year;
        existingSnapshot.Genres = freshSnapshot.Genres;
        existingSnapshot.CastTop = freshSnapshot.CastTop;
        existingSnapshot.CreatedBy = freshSnapshot.CreatedBy;
        existingSnapshot.SeasonsCount = freshSnapshot.SeasonsCount;
        existingSnapshot.ProductionCompanies = freshSnapshot.ProductionCompanies;
        existingSnapshot.OriginCountry = freshSnapshot.OriginCountry;
        existingSnapshot.OriginalLanguage = freshSnapshot.OriginalLanguage;
        existingSnapshot.PosterUrl = freshSnapshot.PosterUrl;
existingSnapshot.Keywords = freshSnapshot.Keywords;
        existingSnapshot.BelongsToCollection = freshSnapshot.BelongsToCollection;
        existingSnapshot.FetchedAt = DateTime.UtcNow;

     await _dbContext.SaveChangesAsync(cancellationToken);

      // Update memory cache
 var cacheKey = BuildCacheKey(existingSnapshot.TmdbId, existingSnapshot.TitleType, existingSnapshot.LanguageCode);
   _memoryCache.Set(cacheKey, existingSnapshot, MemoryCacheTtl);

        _logger.LogInformation("Snapshot refreshed: {Id}", existingSnapshot.Id);

    return existingSnapshot;
    }

    private static string BuildCacheKey(long tmdbId, TitleType titleType, string languageCode)
    {
 return $"snapshot:{titleType.ToString().ToLower()}:{tmdbId}:{languageCode}";
    }

    #endregion
}
