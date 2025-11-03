using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace CineQuizAI.Web.Localization.JsonLocalization;

internal sealed class JsonStringLocalizer : IJsonStringLocalizer
{
    private readonly string _resource;
    private readonly IMemoryCache _cache;
    private readonly JsonLocalizationOptions _opts;

    public JsonStringLocalizer(string resourceName, IMemoryCache cache, JsonLocalizationOptions opts)
    {
        _resource = resourceName;
        _cache = cache;
        _opts = opts;
    }

    public string this[string key] => GetString(key) ?? key;

    public string Format(string key, params object[] args)
    {
        var s = this[key];
        return string.Format(CultureInfo.CurrentUICulture, s, args);
    }

    private string? GetString(string key)
    {
        var culture = CultureInfo.CurrentUICulture;
        var value = GetFromCultureChain(key, culture);
        if (value is not null) return value;

        // fallback to neutral culture (e.g., sv from sv-SE)
        if (culture.Name.Contains('-'))
        {
            var neutral = new CultureInfo(culture.TwoLetterISOLanguageName);
            value = GetFromCultureChain(key, neutral);
            if (value is not null) return value;
        }

        // final fallback
        return GetFromCultureChain(key, new CultureInfo(_opts.FallbackCulture));
    }

    private string? GetFromCultureChain(string key, CultureInfo culture)
    {
        var dict = LoadDictionary(culture);
        if (dict is null) return null;

        // support for "Labels:Password" -> nested lookup
        if (!key.Contains(':'))
        {
            if (dict is Dictionary<string, object> objDict && objDict.TryGetValue(key, out var v))
                return v as string;
            if (dict is Dictionary<string, string> strDict && strDict.TryGetValue(key, out var sv))
                return sv;
            return null;
        }

        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return null;

        // go down levels
        object? cur = dict;
        foreach (var part in parts)
        {
            if (cur is Dictionary<string, object> obj && obj.TryGetValue(part, out var next))
            {
                cur = next;
            }
            else if (cur is Dictionary<string, string> leaf && leaf.TryGetValue(part, out var val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        return cur as string;
    }

    // Recursively convert JsonElement to Dictionary<string, object> or string
    private static object? ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object>();
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = ConvertJsonElement(prop.Value)!;
                }
                return dict;
            case JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(ConvertJsonElement(item));
                }
                return list;
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var l)) return l;
                if (element.TryGetDouble(out var d)) return d;
                return element.GetRawText();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return element.GetRawText();
        }
    }

    // Loads and caches JSON as dictionary
    private object? LoadDictionary(CultureInfo culture)
    {
        var cacheKey = $"jsonloc::{_resource}::{culture.Name}";
        if (_cache.TryGetValue<object>(cacheKey, out var cached))
            return cached;

        // Look for [ResourcesPath]/strings.{culture}.json (e.g., Localization/strings.en.json)
        string path = Path.Combine(_opts.ResourcesPath, $"{_opts.FileName}.{culture.Name}.json");
        if (!File.Exists(path) && culture.Name.Contains('-'))
        {
            path = Path.Combine(_opts.ResourcesPath, $"{_opts.FileName}.{culture.TwoLetterISOLanguageName}.json");
        }

        Debug.WriteLine($"[JsonStringLocalizer] Looking for: {path} Exists: {File.Exists(path)} Culture: {culture.Name}");
        Console.WriteLine($"[JsonStringLocalizer] Looking for: {path} Exists: {File.Exists(path)} Culture: {culture.Name}");

        if (!File.Exists(path))
        {
            var nullValue = (object?)null;
            _cache.Set(cacheKey, nullValue!, TimeSpan.FromMinutes(5));
            return null;
        }

        // Read JSON as UTF-8 to support åäö and other Unicode characters
        var json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        object? result = null;

        // If a resource name (section) is provided, use that sub-object as the root
        if (!string.IsNullOrEmpty(_resource) && root.ValueKind == JsonValueKind.Object && root.TryGetProperty(_resource, out var sectionElem) && sectionElem.ValueKind == JsonValueKind.Object)
        {
            result = ConvertJsonElement(sectionElem);
        }
        else
        {
            result = ConvertJsonElement(root);
        }

        if (result != null)
        {
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        }

        return result;
    }
}
