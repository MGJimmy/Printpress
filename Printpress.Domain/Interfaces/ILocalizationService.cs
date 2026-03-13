namespace Printpress.Domain;

public interface ILocalizationService
{
    /// <summary>Returns the localized string for <paramref name="key"/> in the request language (defaults to Arabic).</summary>
    string Get(string key);

    /// <summary>Returns the localized string with positional placeholders {0}, {1} … replaced.</summary>
    string Get(string key, params object[] args);
}
