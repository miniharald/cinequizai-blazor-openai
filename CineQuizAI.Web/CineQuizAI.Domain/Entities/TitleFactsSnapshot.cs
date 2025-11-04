using CineQuizAI.Domain.Enums;
using CineQuizAI.Domain.ValueObjects;

namespace CineQuizAI.Domain.Entities;

/// <summary>
/// Snapshot of TMDb data for a specific title and language
/// </summary>
public class TitleFactsSnapshot
{
    public Guid Id { get; set; }
    public long TmdbId { get; set; }
    public TitleType TitleType { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int Year { get; set; }
    
    // JSONB fields (will be stored as JSON in PostgreSQL)
    public List<string> Genres { get; set; } = new();
    public List<CastMember> CastTop { get; set; } = new();
    public List<string> CreatedBy { get; set; } = new(); // TV only
  public int? SeasonsCount { get; set; } // TV only
    public List<string> ProductionCompanies { get; set; } = new();
    public List<string> OriginCountry { get; set; } = new();
 public string OriginalLanguage { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public List<string> Keywords { get; set; } = new();
 public Collection? BelongsToCollection { get; set; } // Movie only
    
  public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
 
    // Navigation property
    public ICollection<QuizSession> QuizSessions { get; set; } = new List<QuizSession>();
}
