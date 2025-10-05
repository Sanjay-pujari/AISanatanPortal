using AISanatanPortal.API.Services;

namespace AISanatanPortal.API.Services;

public interface ILanguageDetectionService
{
    Task<string> DetectLanguageFromTextAsync(string text);
    Task<string> GetUserPreferredLanguageAsync(HttpContext context);
    Task<bool> IsLanguageSupportedAsync(string languageCode);
    Task<string> GetFallbackLanguageAsync(string preferredLanguage);
}

public class LanguageDetectionService : ILanguageDetectionService
{
    private readonly ITranslationService _translationService;
    private readonly ILogger<LanguageDetectionService> _logger;
    private readonly Dictionary<string, List<string>> _scriptToLanguage;

    public LanguageDetectionService(
        ITranslationService translationService,
        ILogger<LanguageDetectionService> logger)
    {
        _translationService = translationService;
        _logger = logger;
        _scriptToLanguage = InitializeScriptToLanguageMapping();
    }

    public async Task<string> DetectLanguageFromTextAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "en";

        try
        {
            // First try the translation service's detection
            var detectedLanguage = await _translationService.DetectLanguageAsync(text);
            if (detectedLanguage != "en" && await IsLanguageSupportedAsync(detectedLanguage))
            {
                return detectedLanguage;
            }

            // Fallback to script-based detection
            return DetectLanguageByScript(text);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Language detection failed, defaulting to English");
            return "en";
        }
    }

    public async Task<string> GetUserPreferredLanguageAsync(HttpContext context)
    {
        // Priority order: Query param > Header > Session > Cookie > Accept-Language > Auto-detect
        var queryLang = context.Request.Query["lang"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(queryLang) && await IsLanguageSupportedAsync(queryLang))
        {
            return NormalizeLanguageCode(queryLang);
        }

        var headerLang = context.Request.Headers["X-Language"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerLang) && await IsLanguageSupportedAsync(headerLang))
        {
            return NormalizeLanguageCode(headerLang);
        }

        // Check session
        var sessionLang = context.Session.GetString("Language");
        if (!string.IsNullOrWhiteSpace(sessionLang) && await IsLanguageSupportedAsync(sessionLang))
        {
            return sessionLang;
        }

        // Check cookie
        var cookieLang = context.Request.Cookies["Language"];
        if (!string.IsNullOrWhiteSpace(cookieLang) && await IsLanguageSupportedAsync(cookieLang))
        {
            return cookieLang;
        }

        // Parse Accept-Language header
        var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            var parsedLang = ParseAcceptLanguageHeader(acceptLanguage);
            if (await IsLanguageSupportedAsync(parsedLang))
            {
                return parsedLang;
            }
        }

        // Auto-detect from user agent or other indicators
        return await AutoDetectLanguageAsync(context);
    }

    public async Task<bool> IsLanguageSupportedAsync(string languageCode)
    {
        var supportedLanguages = await _translationService.GetSupportedLanguagesAsync();
        return supportedLanguages.ContainsKey(languageCode.ToLower());
    }

    public async Task<string> GetFallbackLanguageAsync(string preferredLanguage)
    {
        // Language fallback hierarchy
        var fallbackMap = new Dictionary<string, string>
        {
            ["hi"] = "en", // Hindi -> English
            ["sa"] = "hi", // Sanskrit -> Hindi -> English
            ["gu"] = "hi", // Gujarati -> Hindi -> English
            ["ta"] = "en", // Tamil -> English
            ["te"] = "en", // Telugu -> English
            ["bn"] = "en", // Bengali -> English
            ["mr"] = "hi", // Marathi -> Hindi -> English
            ["kn"] = "en", // Kannada -> English
            ["ml"] = "en", // Malayalam -> English
            ["pa"] = "hi", // Punjabi -> Hindi -> English
            ["or"] = "en", // Odia -> English
            ["as"] = "en"  // Assamese -> English
        };

        var normalizedLang = preferredLanguage.ToLower();
        
        if (fallbackMap.ContainsKey(normalizedLang))
        {
            var fallback = fallbackMap[normalizedLang];
            if (await IsLanguageSupportedAsync(fallback))
            {
                return fallback;
            }
        }

        return "en"; // Ultimate fallback
    }

    private string DetectLanguageByScript(string text)
    {
        foreach (var script in _scriptToLanguage)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(text, script.Key))
            {
                return script.Value.FirstOrDefault() ?? "en";
            }
        }

        return "en";
    }

    private string ParseAcceptLanguageHeader(string acceptLanguage)
    {
        // Parse "en-US,en;q=0.9,hi;q=0.8" format
        var languages = acceptLanguage.Split(',')
            .Select(lang => lang.Split(';')[0].Trim())
            .ToList();

        foreach (var lang in languages)
        {
            var normalizedLang = NormalizeLanguageCode(lang);
            if (normalizedLang != "en")
            {
                return normalizedLang;
            }
        }

        return "en";
    }

    private async Task<string> AutoDetectLanguageAsync(HttpContext context)
    {
        // Could implement more sophisticated detection based on:
        // - User's location (IP geolocation)
        // - Previous requests
        // - User profile
        // - Browser language settings
        
        await Task.CompletedTask; // Placeholder for future implementation
        return "en";
    }

    private string NormalizeLanguageCode(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "en";

        var normalizedCode = languageCode.Split('-')[0].ToLower();

        var languageMapping = new Dictionary<string, string>
        {
            ["hi"] = "hi", ["hin"] = "hi", ["hindi"] = "hi",
            ["sa"] = "sa", ["san"] = "sa", ["sanskrit"] = "sa", ["skr"] = "sa",
            ["gu"] = "gu", ["guj"] = "gu", ["gujarati"] = "gu",
            ["ta"] = "ta", ["tam"] = "ta", ["tamil"] = "ta",
            ["te"] = "te", ["tel"] = "te", ["telugu"] = "te",
            ["bn"] = "bn", ["ben"] = "bn", ["bengali"] = "bn",
            ["mr"] = "mr", ["mar"] = "mr", ["marathi"] = "mr",
            ["kn"] = "kn", ["kan"] = "kn", ["kannada"] = "kn",
            ["ml"] = "ml", ["mal"] = "ml", ["malayalam"] = "ml",
            ["pa"] = "pa", ["pan"] = "pa", ["punjabi"] = "pa",
            ["or"] = "or", ["odi"] = "or", ["odia"] = "or", ["oriya"] = "or",
            ["as"] = "as", ["asm"] = "as", ["assamese"] = "as"
        };

        return languageMapping.TryGetValue(normalizedCode, out var mappedCode) ? mappedCode : "en";
    }

    private Dictionary<string, List<string>> InitializeScriptToLanguageMapping()
    {
        return new Dictionary<string, List<string>>
        {
            // Devanagari script (Hindi, Sanskrit, Marathi, etc.)
            [@"[\u0900-\u097F]"] = new List<string> { "hi", "sa", "mr" },
            
            // Tamil script
            [@"[\u0B80-\u0BFF]"] = new List<string> { "ta" },
            
            // Telugu script
            [@"[\u0C00-\u0C7F]"] = new List<string> { "te" },
            
            // Bengali script
            [@"[\u0980-\u09FF]"] = new List<string> { "bn", "as" },
            
            // Gujarati script
            [@"[\u0A80-\u0AFF]"] = new List<string> { "gu" },
            
            // Kannada script
            [@"[\u0C80-\u0CFF]"] = new List<string> { "kn" },
            
            // Malayalam script
            [@"[\u0D00-\u0D7F]"] = new List<string> { "ml" },
            
            // Gurmukhi script (Punjabi)
            [@"[\u0A00-\u0A7F]"] = new List<string> { "pa" },
            
            // Odia script
            [@"[\u0B00-\u0B7F]"] = new List<string> { "or" },
            
            // Arabic script
            [@"[\u0600-\u06FF]"] = new List<string> { "ar" },
            
            // Chinese script
            [@"[\u4E00-\u9FFF]"] = new List<string> { "zh" },
            
            // Japanese script
            [@"[\u3040-\u309F\u30A0-\u30FF]"] = new List<string> { "ja" }
        };
    }
}

