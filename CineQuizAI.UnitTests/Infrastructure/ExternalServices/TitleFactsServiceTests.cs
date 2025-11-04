using CineQuizAI.Application.Abstractions.ExternalServices;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using CineQuizAI.Infrastructure.Data;
using CineQuizAI.Infrastructure.ExternalServices.Tmdb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CineQuizAI.UnitTests.Infrastructure.ExternalServices;

public class TitleFactsServiceTests
{
    private readonly Mock<ITmdbClient> _tmdbClientMock;
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
 private readonly Mock<ILogger<TitleFactsService>> _loggerMock;
    private readonly TitleFactsService _service;

  public TitleFactsServiceTests()
    {
  _tmdbClientMock = new Mock<ITmdbClient>();
        
        // Setup in-memory database
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;
        _dbContext = new AppDbContext(options);

  _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<TitleFactsService>>();

   _service = new TitleFactsService(
     _tmdbClientMock.Object,
            _dbContext,
   _memoryCache,
     _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetOrCreateSnapshotAsync_WhenSnapshotDoesNotExist_ShouldFetchFromTmdb()
    {
        // Arrange
        var tmdbId = 12345L;
  var titleType = TitleType.Movie;
    var languageCode = "sv-SE";
        var expectedSnapshot = CreateTestSnapshot(tmdbId, titleType, languageCode);

  _tmdbClientMock
   .Setup(x => x.GetTitleSnapshotAsync(tmdbId, titleType, languageCode, It.IsAny<CancellationToken>()))
          .ReturnsAsync(expectedSnapshot);

        // Act
  var result = await _service.GetOrCreateSnapshotAsync(tmdbId, titleType, languageCode);

        // Assert
        result.Should().NotBeNull();
  result.TmdbId.Should().Be(tmdbId);
    result.TitleType.Should().Be(titleType);
        result.LanguageCode.Should().Be(languageCode);
  result.Title.Should().Be(expectedSnapshot.Title);

  // Verify it was saved to database
   var dbSnapshot = await _dbContext.TitleFactsSnapshots.FirstOrDefaultAsync();
   dbSnapshot.Should().NotBeNull();
 dbSnapshot!.TmdbId.Should().Be(tmdbId);
    }

    [Fact]
  public async Task GetOrCreateSnapshotAsync_WhenSnapshotExistsAndFresh_ShouldReturnCachedVersion()
    {
// Arrange
        var tmdbId = 12345L;
  var titleType = TitleType.Movie;
  var languageCode = "sv-SE";
        var existingSnapshot = CreateTestSnapshot(tmdbId, titleType, languageCode);
        existingSnapshot.FetchedAt = DateTime.UtcNow; // Fresh snapshot

  _dbContext.TitleFactsSnapshots.Add(existingSnapshot);
     await _dbContext.SaveChangesAsync();

    // Act
var result = await _service.GetOrCreateSnapshotAsync(tmdbId, titleType, languageCode);

        // Assert
   result.Should().NotBeNull();
        result.Id.Should().Be(existingSnapshot.Id);
        
        // TMDb client should not have been called
        _tmdbClientMock.Verify(
      x => x.GetTitleSnapshotAsync(It.IsAny<long>(), It.IsAny<TitleType>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
       Times.Never);
    }

    [Fact]
    public async Task GetOrCreateSnapshotAsync_WhenSnapshotExistsButStale_ShouldRefreshFromTmdb()
    {
     // Arrange
    var tmdbId = 12345L;
  var titleType = TitleType.Movie;
        var languageCode = "sv-SE";
   var staleSnapshot = CreateTestSnapshot(tmdbId, titleType, languageCode);
      staleSnapshot.FetchedAt = DateTime.UtcNow.AddDays(-8); // Stale (>7 days for movies)

   _dbContext.TitleFactsSnapshots.Add(staleSnapshot);
   await _dbContext.SaveChangesAsync();

   var freshSnapshot = CreateTestSnapshot(tmdbId, titleType, languageCode);
      freshSnapshot.Title = "Updated Title";

        _tmdbClientMock
     .Setup(x => x.GetTitleSnapshotAsync(tmdbId, titleType, languageCode, It.IsAny<CancellationToken>()))
       .ReturnsAsync(freshSnapshot);

        // Act
   var result = await _service.GetOrCreateSnapshotAsync(tmdbId, titleType, languageCode);

        // Assert
   result.Should().NotBeNull();
   result.Title.Should().Be("Updated Title");
      
    // TMDb client should have been called to refresh
        _tmdbClientMock.Verify(
       x => x.GetTitleSnapshotAsync(tmdbId, titleType, languageCode, It.IsAny<CancellationToken>()),
  Times.Once);
  }

 [Fact]
  public void ShouldRefreshSnapshot_WhenMovieOlderThan7Days_ShouldReturnTrue()
    {
        // Arrange
 var snapshot = CreateTestSnapshot(12345, TitleType.Movie, "sv-SE");
   snapshot.FetchedAt = DateTime.UtcNow.AddDays(-8);

        // Act
     var shouldRefresh = _service.ShouldRefreshSnapshot(snapshot);

        // Assert
  shouldRefresh.Should().BeTrue();
    }

    [Fact]
    public void ShouldRefreshSnapshot_WhenTvSeriesOlderThan3Days_ShouldReturnTrue()
    {
      // Arrange
        var snapshot = CreateTestSnapshot(12345, TitleType.Tv, "sv-SE");
   snapshot.FetchedAt = DateTime.UtcNow.AddDays(-4);

      // Act
   var shouldRefresh = _service.ShouldRefreshSnapshot(snapshot);

   // Assert
        shouldRefresh.Should().BeTrue();
    }

[Fact]
    public void ShouldRefreshSnapshot_WhenSnapshotFresh_ShouldReturnFalse()
    {
// Arrange
        var snapshot = CreateTestSnapshot(12345, TitleType.Movie, "sv-SE");
  snapshot.FetchedAt = DateTime.UtcNow;

        // Act
   var shouldRefresh = _service.ShouldRefreshSnapshot(snapshot);

 // Assert
  shouldRefresh.Should().BeFalse();
    }

    private TitleFactsSnapshot CreateTestSnapshot(long tmdbId, TitleType titleType, string languageCode)
    {
        return new TitleFactsSnapshot
        {
       Id = Guid.NewGuid(),
     TmdbId = tmdbId,
    TitleType = titleType,
       LanguageCode = languageCode,
     Title = "Test Movie",
   Overview = "Test overview",
    Year = 2024,
            Genres = new List<string> { "Action", "Drama" },
        CastTop = new List<CineQuizAI.Domain.ValueObjects.CastMember>
       {
new() { Name = "Actor 1", Character = "Character 1" }
       },
            CreatedBy = new List<string>(),
 ProductionCompanies = new List<string> { "Test Studio" },
            OriginCountry = new List<string> { "US" },
      OriginalLanguage = "en",
            Keywords = new List<string> { "test", "movie" },
   FetchedAt = DateTime.UtcNow
 };
    }
}
