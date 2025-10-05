using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Services;

public interface ITranslationService
{
    Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "en");
    Task<T> TranslateObjectAsync<T>(T obj, string targetLanguage, string sourceLanguage = "en") where T : class;
    Task<ApiResponse<T>> TranslateApiResponseAsync<T>(ApiResponse<T> response, string targetLanguage, string sourceLanguage = "en") where T : class;
    Task<bool> IsTranslationRequiredAsync(string targetLanguage, string sourceLanguage = "en");
    Task<Dictionary<string, string>> GetSupportedLanguagesAsync();
    Task<string> DetectLanguageAsync(string text);
}

public class TranslationService : ITranslationService
{
    private readonly ILogger<TranslationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, LanguageInfo> _supportedLanguages;

    public TranslationService(
        ILogger<TranslationService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
        _supportedLanguages = InitializeSupportedLanguages();
    }

    public async Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "en")
    {
        if (string.IsNullOrWhiteSpace(text) || !await IsTranslationRequiredAsync(targetLanguage, sourceLanguage))
        {
            return text;
        }

        try
        {
            // For now, using a simple translation mapping
            // In production, you'd integrate with Azure Translator, Google Translate, or similar
            return await TranslateWithMappingAsync(text, targetLanguage, sourceLanguage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
            return text; // Return original text if translation fails
        }
    }

    public async Task<T> TranslateObjectAsync<T>(T obj, string targetLanguage, string sourceLanguage = "en") where T : class
    {
        if (obj == null || !await IsTranslationRequiredAsync(targetLanguage, sourceLanguage))
        {
            return obj;
        }

        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var translatedJson = await TranslateJsonAsync(json, targetLanguage, sourceLanguage);
            return System.Text.Json.JsonSerializer.Deserialize<T>(translatedJson) ?? obj;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating object from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
            return obj;
        }
    }

    public async Task<ApiResponse<T>> TranslateApiResponseAsync<T>(ApiResponse<T> response, string targetLanguage, string sourceLanguage = "en") where T : class
    {
        if (response == null || !await IsTranslationRequiredAsync(targetLanguage, sourceLanguage))
        {
            return response;
        }

        try
        {
            var translatedResponse = new ApiResponse<T>
            {
                Success = response.Success,
                Data = response.Data,
                TotalCount = response.TotalCount,
                CurrentPage = response.CurrentPage,
                TotalPages = response.TotalPages,
                Errors = response.Errors
            };

            // Translate the message
            translatedResponse.Message = await TranslateTextAsync(response.Message, targetLanguage, sourceLanguage);

            // Translate error messages
            if (response.Errors?.Any() == true)
            {
                var translatedErrors = new List<string>();
                foreach (var error in response.Errors)
                {
                    translatedErrors.Add(await TranslateTextAsync(error, targetLanguage, sourceLanguage));
                }
                translatedResponse.Errors = translatedErrors;
            }

            // Translate the data object if it contains translatable text
            if (response.Data != null)
            {
                translatedResponse.Data = await TranslateObjectAsync(response.Data, targetLanguage, sourceLanguage);
            }

            return translatedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating API response from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
            return response;
        }
    }

    public async Task<bool> IsTranslationRequiredAsync(string targetLanguage, string sourceLanguage = "en")
    {
        await Task.CompletedTask; // Async placeholder
        return !string.IsNullOrWhiteSpace(targetLanguage) && 
               targetLanguage.ToLower() != sourceLanguage.ToLower() &&
               _supportedLanguages.ContainsKey(targetLanguage.ToLower());
    }

    public async Task<Dictionary<string, string>> GetSupportedLanguagesAsync()
    {
        await Task.CompletedTask;
        return _supportedLanguages.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Name
        );
    }

    public async Task<string> DetectLanguageAsync(string text)
    {
        await Task.CompletedTask; // In production, use Azure Translator or similar service
        
        // Simple language detection based on character patterns
        if (string.IsNullOrWhiteSpace(text))
            return "en";

        // Check for Devanagari script (Hindi/Sanskrit)
        if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0900-\u097F]"))
            return "hi";

        // Check for Arabic script
        if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0600-\u06FF]"))
            return "ar";

        // Default to English
        return "en";
    }

    private async Task<string> TranslateWithMappingAsync(string text, string targetLanguage, string sourceLanguage)
    {
        // Simple translation mapping for common terms
        // In production, replace with actual translation service
        var translations = GetTranslationMappings();
        
        var targetLang = targetLanguage.ToLower();
        var sourceLang = sourceLanguage.ToLower();

        if (translations.ContainsKey(sourceLang) && translations[sourceLang].ContainsKey(targetLang))
        {
            var sourceMappings = translations[sourceLang][targetLang];
            var translatedText = text;

            foreach (var mapping in sourceMappings)
            {
                translatedText = translatedText.Replace(mapping.Key, mapping.Value, StringComparison.OrdinalIgnoreCase);
            }

            return translatedText;
        }

        return text; // Return original if no translation mapping found
    }

    private async Task<string> TranslateJsonAsync(string json, string targetLanguage, string sourceLanguage)
    {
        // Parse JSON and translate string values
        using var document = System.Text.Json.JsonDocument.Parse(json);
        var translatedJson = await TranslateJsonElementAsync(document.RootElement, targetLanguage, sourceLanguage);
        return System.Text.Json.JsonSerializer.Serialize(translatedJson);
    }

    private async Task<System.Text.Json.JsonElement> TranslateJsonElementAsync(System.Text.Json.JsonElement element, string targetLanguage, string sourceLanguage)
    {
        switch (element.ValueKind)
        {
            case System.Text.Json.JsonValueKind.String:
                var translatedString = await TranslateTextAsync(element.GetString() ?? "", targetLanguage, sourceLanguage);
                return CreateJsonElementFromString(translatedString);

            case System.Text.Json.JsonValueKind.Object:
                var translatedObject = new Dictionary<string, object>();
                foreach (var property in element.EnumerateObject())
                {
                    var translatedValue = await TranslateJsonElementAsync(property.Value, targetLanguage, sourceLanguage);
                    translatedObject[property.Name] = translatedValue;
                }
                return CreateJsonElementFromSerialized(System.Text.Json.JsonSerializer.Serialize(translatedObject));

            case System.Text.Json.JsonValueKind.Array:
                var translatedArray = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    var translatedItem = await TranslateJsonElementAsync(item, targetLanguage, sourceLanguage);
                    translatedArray.Add(translatedItem);
                }
                return CreateJsonElementFromSerialized(System.Text.Json.JsonSerializer.Serialize(translatedArray));

            default:
                return element;
        }
    }

    private static System.Text.Json.JsonElement CreateJsonElementFromString(string value)
    {
        using var doc = System.Text.Json.JsonDocument.Parse($"\"{value}\"");
        return doc.RootElement.Clone();
    }

    private static System.Text.Json.JsonElement CreateJsonElementFromSerialized(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private Dictionary<string, LanguageInfo> InitializeSupportedLanguages()
    {
        return new Dictionary<string, LanguageInfo>
        {
            ["en"] = new LanguageInfo { Code = "en", Name = "English", NativeName = "English" },
            ["hi"] = new LanguageInfo { Code = "hi", Name = "Hindi", NativeName = "हिन्दी" },
            ["sa"] = new LanguageInfo { Code = "sa", Name = "Sanskrit", NativeName = "संस्कृतम्" },
            ["gu"] = new LanguageInfo { Code = "gu", Name = "Gujarati", NativeName = "ગુજરાતી" },
            ["ta"] = new LanguageInfo { Code = "ta", Name = "Tamil", NativeName = "தமிழ்" },
            ["te"] = new LanguageInfo { Code = "te", Name = "Telugu", NativeName = "తెలుగు" },
            ["bn"] = new LanguageInfo { Code = "bn", Name = "Bengali", NativeName = "বাংলা" },
            ["mr"] = new LanguageInfo { Code = "mr", Name = "Marathi", NativeName = "मराठी" },
            ["kn"] = new LanguageInfo { Code = "kn", Name = "Kannada", NativeName = "ಕನ್ನಡ" },
            ["ml"] = new LanguageInfo { Code = "ml", Name = "Malayalam", NativeName = "മലയാളം" },
            ["pa"] = new LanguageInfo { Code = "pa", Name = "Punjabi", NativeName = "ਪੰਜਾਬੀ" },
            ["or"] = new LanguageInfo { Code = "or", Name = "Odia", NativeName = "ଓଡ଼ିଆ" },
            ["as"] = new LanguageInfo { Code = "as", Name = "Assamese", NativeName = "অসমীয়া" }
        };
    }

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetTranslationMappings()
    {
        return new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
        {
            ["en"] = new Dictionary<string, Dictionary<string, string>>
            {
                ["hi"] = new Dictionary<string, string>
                {
                    ["Welcome"] = "स्वागत",
                    ["Login"] = "लॉग इन",
                    ["Register"] = "रजिस्टर",
                    ["Password"] = "पासवर्ड",
                    ["Email"] = "ईमेल",
                    ["Name"] = "नाम",
                    ["Success"] = "सफलता",
                    ["Error"] = "त्रुटि",
                    ["Loading"] = "लोड हो रहा है",
                    ["Save"] = "सेव करें",
                    ["Cancel"] = "रद्द करें",
                    ["Delete"] = "हटाएं",
                    ["Edit"] = "संपादित करें",
                    ["Search"] = "खोजें",
                    ["Filter"] = "फिल्टर",
                    ["Vedas"] = "वेद",
                    ["Puranas"] = "पुराण",
                    ["Temples"] = "मंदिर",
                    ["Spiritual"] = "आध्यात्मिक",
                    ["Guidance"] = "मार्गदर्शन",
                    ["Meditation"] = "ध्यान",
                    ["Prayer"] = "प्रार्थना",
                    ["Festival"] = "त्योहार",
                    ["Ritual"] = "अनुष्ठान",
                    ["Philosophy"] = "दर्शन"
                },
                ["sa"] = new Dictionary<string, string>
                {
                    ["Welcome"] = "स्वागतम्",
                    ["Vedas"] = "वेदः",
                    ["Spiritual"] = "आध्यात्मिकः",
                    ["Guidance"] = "मार्गदर्शनम्",
                    ["Meditation"] = "ध्यानम्",
                    ["Prayer"] = "प्रार्थना",
                    ["Temple"] = "देवालयः",
                    ["Festival"] = "उत्सवः",
                    ["Ritual"] = "कर्मकाण्डम्",
                    ["Philosophy"] = "दर्शनम्"
                }
            },
            ["hi"] = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
                {
                    ["स्वागत"] = "Welcome",
                    ["लॉग इन"] = "Login",
                    ["रजिस्टर"] = "Register",
                    ["पासवर्ड"] = "Password",
                    ["ईमेल"] = "Email",
                    ["नाम"] = "Name",
                    ["सफलता"] = "Success",
                    ["त्रुटि"] = "Error",
                    ["लोड हो रहा है"] = "Loading",
                    ["सेव करें"] = "Save",
                    ["रद्द करें"] = "Cancel",
                    ["हटाएं"] = "Delete",
                    ["संपादित करें"] = "Edit",
                    ["खोजें"] = "Search",
                    ["फिल्टर"] = "Filter",
                    ["वेद"] = "Vedas",
                    ["पुराण"] = "Puranas",
                    ["मंदिर"] = "Temples",
                    ["आध्यात्मिक"] = "Spiritual",
                    ["मार्गदर्शन"] = "Guidance",
                    ["ध्यान"] = "Meditation",
                    ["प्रार्थना"] = "Prayer",
                    ["त्योहार"] = "Festival",
                    ["अनुष्ठान"] = "Ritual",
                    ["दर्शन"] = "Philosophy"
                }
            }
        };
    }
}

public class LanguageInfo
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
}

