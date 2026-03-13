using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Printpress.Domain;

namespace Printpress.Infrastructure;

/// <summary>
/// Reads embedded JSON files from Printpress.Domain (Shared/Localization/**) and returns
/// translated strings keyed by "schema.key".  Language is resolved from the request
/// Accept-Language header; falls back to Arabic ("ar").
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private const string DefaultLang = "ar";

    // lang -> (key -> value)
    private static readonly Dictionary<string, Dictionary<string, string>> _cache;

    static LocalizationService()
    {
        _cache = LoadFromAssembly(typeof(LocalizationKeys).Assembly);
    }

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LocalizationService> _logger;

    public LocalizationService(IHttpContextAccessor httpContextAccessor, ILogger<LocalizationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string Get(string key) => Get(key, Array.Empty<object>());

    public string Get(string key, params object[] args)
    {
        var lang = ResolveLanguage();

        if (_cache.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out var value))
            return args.Length > 0 ? string.Format(value, args) : value;

        // Fallback to default language
        if (lang != DefaultLang && _cache.TryGetValue(DefaultLang, out var fallback) && fallback.TryGetValue(key, out var fallbackValue))
            return args.Length > 0 ? string.Format(fallbackValue, args) : fallbackValue;

        _logger.LogWarning("Localization key '{Key}' not found for language '{Lang}'", key, lang);
        return key; // return key itself as last resort
    }

    // ─── helpers ────────────────────────────────────────────────────────────

    private string ResolveLanguage()
    {
        var acceptLang = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(acceptLang)) return DefaultLang;

        // Accept-Language can be "ar,en;q=0.9" – take the first tag
        var primary = acceptLang.Split(',')[0].Trim().Split(';')[0].Trim().ToLowerInvariant();

        // Normalise: "ar-EG" -> "ar", "en-US" -> "en"
        if (primary.Contains('-')) primary = primary.Split('-')[0];

        return _cache.ContainsKey(primary) ? primary : DefaultLang;
    }

    private static Dictionary<string, Dictionary<string, string>> LoadFromAssembly(Assembly assembly)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            // Expected pattern: Printpress.Domain.Shared.Localization.*.json
            if (!resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;
            if (!resourceName.Contains(".Localization.")) continue;

            // Extract language from filename: "…ar.json" -> "ar", "…en.json" -> "en"
            var fileName = resourceName.Split('.')[^2]; // second-to-last segment before .json
            var lang = fileName.ToLowerInvariant();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null) continue;

            var doc = JsonDocument.Parse(stream);
            if (!result.ContainsKey(lang))
                result[lang] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in doc.RootElement.EnumerateObject())
                result[lang][prop.Name] = prop.Value.GetString() ?? string.Empty;
        }

        return result;
    }
}
