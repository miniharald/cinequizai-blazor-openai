using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CineQuizAI.Web.Localization.JsonLocalization;

public sealed class JsonStringLocalizerFactory : IJsonStringLocalizerFactory
{
    private readonly IMemoryCache _cache;
    private readonly JsonLocalizationOptions _opts;

    public JsonStringLocalizerFactory(IMemoryCache cache, IOptions<JsonLocalizationOptions> opts)
    {
        _cache = cache;
        _opts = opts.Value;
    }

    public IJsonStringLocalizer Create(string resourceName)
        => new JsonStringLocalizer(resourceName, _cache, _opts);
}
