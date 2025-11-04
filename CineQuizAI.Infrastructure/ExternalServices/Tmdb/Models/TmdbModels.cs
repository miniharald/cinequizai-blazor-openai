using System.Text.Json.Serialization;

namespace CineQuizAI.Infrastructure.ExternalServices.Tmdb.Models;

/// <summary>
/// TMDb Movie Details Response
/// </summary>
public class TmdbMovieDetails
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

  [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("genres")]
    public List<TmdbGenre> Genres { get; set; } = new();

    [JsonPropertyName("production_companies")]
    public List<TmdbProductionCompany> ProductionCompanies { get; set; } = new();

    [JsonPropertyName("production_countries")]
    public List<TmdbProductionCountry> ProductionCountries { get; set; } = new();

  [JsonPropertyName("keywords")]
    public TmdbKeywordsContainer? Keywords { get; set; }

    [JsonPropertyName("credits")]
    public TmdbCredits? Credits { get; set; }

    [JsonPropertyName("belongs_to_collection")]
    public TmdbCollection? BelongsToCollection { get; set; }
}

/// <summary>
/// TMDb TV Series Details Response
/// </summary>
public class TmdbTvDetails
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
 public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("genres")]
    public List<TmdbGenre> Genres { get; set; } = new();

    [JsonPropertyName("created_by")]
    public List<TmdbCreator> CreatedBy { get; set; } = new();

    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("production_companies")]
    public List<TmdbProductionCompany> ProductionCompanies { get; set; } = new();

    [JsonPropertyName("origin_country")]
    public List<string> OriginCountry { get; set; } = new();

    [JsonPropertyName("keywords")]
    public TmdbKeywordsContainer? Keywords { get; set; }

    [JsonPropertyName("credits")]
    public TmdbCredits? Credits { get; set; }
}

/// <summary>
/// TMDb Genre
/// </summary>
public class TmdbGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Production Company
/// </summary>
public class TmdbProductionCompany
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

[JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Production Country
/// </summary>
public class TmdbProductionCountry
{
    [JsonPropertyName("iso_3166_1")]
    public string Iso31661 { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Creator (for TV series)
/// </summary>
public class TmdbCreator
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Keywords Container
/// </summary>
public class TmdbKeywordsContainer
{
    [JsonPropertyName("keywords")]
    public List<TmdbKeyword>? Keywords { get; set; }

    [JsonPropertyName("results")]
public List<TmdbKeyword>? Results { get; set; }
}

/// <summary>
/// TMDb Keyword
/// </summary>
public class TmdbKeyword
{
  [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Credits (cast and crew)
/// </summary>
public class TmdbCredits
{
  [JsonPropertyName("cast")]
    public List<TmdbCastMember> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<TmdbCrewMember> Crew { get; set; } = new();
}

/// <summary>
/// TMDb Cast Member
/// </summary>
public class TmdbCastMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("character")]
    public string Character { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}

/// <summary>
/// TMDb Crew Member
/// </summary>
public class TmdbCrewMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

 [JsonPropertyName("job")]
    public string Job { get; set; } = string.Empty;

    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;
}

/// <summary>
/// TMDb Collection
/// </summary>
public class TmdbCollection
{
  [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
}

/// <summary>
/// TMDb Search Response
/// </summary>
public class TmdbSearchResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TmdbSearchItem> Results { get; set; } = new();

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

  [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}

/// <summary>
/// TMDb Search Item
/// </summary>
public class TmdbSearchItem
{
    [JsonPropertyName("id")]
public long Id { get; set; }

 [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }
}
