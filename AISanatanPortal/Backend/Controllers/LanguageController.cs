using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Services;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguageController : ControllerBase
{
    private readonly ITranslationService _translationService;
    private readonly ILanguageDetectionService _languageDetectionService;
    private readonly ILogger<LanguageController> _logger;

    public LanguageController(
        ITranslationService translationService,
        ILanguageDetectionService languageDetectionService,
        ILogger<LanguageController> logger)
    {
        _translationService = translationService;
        _languageDetectionService = languageDetectionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all supported languages
    /// </summary>
    [HttpGet("supported")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string>>>> GetSupportedLanguages()
    {
        try
        {
            var languages = await _translationService.GetSupportedLanguagesAsync();
            return Ok(new ApiResponse<Dictionary<string, string>>
            {
                Success = true,
                Message = "Supported languages retrieved successfully",
                Data = languages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supported languages");
            return StatusCode(500, new ApiResponse<Dictionary<string, string>>
            {
                Success = false,
                Message = "Error retrieving supported languages"
            });
        }
    }

    /// <summary>
    /// Detect language from text
    /// </summary>
    [HttpPost("detect")]
    public async Task<ActionResult<ApiResponse<LanguageDetectionResult>>> DetectLanguage([FromBody] LanguageDetectionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new ApiResponse<LanguageDetectionResult>
                {
                    Success = false,
                    Message = "Text is required for language detection"
                });
            }

            var detectedLanguage = await _languageDetectionService.DetectLanguageFromTextAsync(request.Text);
            var isSupported = await _languageDetectionService.IsLanguageSupportedAsync(detectedLanguage);

            var result = new LanguageDetectionResult
            {
                DetectedLanguage = detectedLanguage,
                IsSupported = isSupported,
                Confidence = CalculateConfidence(request.Text, detectedLanguage),
                FallbackLanguage = await _languageDetectionService.GetFallbackLanguageAsync(detectedLanguage)
            };

            return Ok(new ApiResponse<LanguageDetectionResult>
            {
                Success = true,
                Message = "Language detected successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting language");
            return StatusCode(500, new ApiResponse<LanguageDetectionResult>
            {
                Success = false,
                Message = "Error detecting language"
            });
        }
    }

    /// <summary>
    /// Get user's preferred language based on request context
    /// </summary>
    [HttpGet("preferred")]
    public async Task<ActionResult<ApiResponse<UserLanguagePreference>>> GetUserPreferredLanguage()
    {
        try
        {
            var preferredLanguage = await _languageDetectionService.GetUserPreferredLanguageAsync(HttpContext);
            var isSupported = await _languageDetectionService.IsLanguageSupportedAsync(preferredLanguage);

            var preference = new UserLanguagePreference
            {
                PreferredLanguage = preferredLanguage,
                IsSupported = isSupported,
                FallbackLanguage = await _languageDetectionService.GetFallbackLanguageAsync(preferredLanguage),
                DetectionMethod = GetDetectionMethod(HttpContext)
            };

            return Ok(new ApiResponse<UserLanguagePreference>
            {
                Success = true,
                Message = "User language preference retrieved successfully",
                Data = preference
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user language preference");
            return StatusCode(500, new ApiResponse<UserLanguagePreference>
            {
                Success = false,
                Message = "Error getting user language preference"
            });
        }
    }

    /// <summary>
    /// Set user's language preference
    /// </summary>
    [HttpPost("preference")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> SetLanguagePreference([FromBody] SetLanguagePreferenceRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LanguageCode))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Language code is required"
                });
            }

            var isSupported = await _languageDetectionService.IsLanguageSupportedAsync(request.LanguageCode);
            if (!isSupported)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Language '{request.LanguageCode}' is not supported"
                });
            }

            // Set language preference in session and cookie
            HttpContext.Session.SetString("Language", request.LanguageCode);
            
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            };
            
            Response.Cookies.Append("Language", request.LanguageCode, cookieOptions);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Language preference set successfully",
                Data = request.LanguageCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting language preference");
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error setting language preference"
            });
        }
    }

    /// <summary>
    /// Test translation functionality
    /// </summary>
    [HttpPost("test-translation")]
    public async Task<ActionResult<ApiResponse<TranslationTestResult>>> TestTranslation([FromBody] TranslationTestRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text) || string.IsNullOrWhiteSpace(request.TargetLanguage))
            {
                return BadRequest(new ApiResponse<TranslationTestResult>
                {
                    Success = false,
                    Message = "Text and target language are required"
                });
            }

            var isSupported = await _languageDetectionService.IsLanguageSupportedAsync(request.TargetLanguage);
            if (!isSupported)
            {
                return BadRequest(new ApiResponse<TranslationTestResult>
                {
                    Success = false,
                    Message = $"Target language '{request.TargetLanguage}' is not supported"
                });
            }

            var translatedText = await _translationService.TranslateTextAsync(
                request.Text, 
                request.TargetLanguage, 
                request.SourceLanguage ?? "en");

            var result = new TranslationTestResult
            {
                OriginalText = request.Text,
                TranslatedText = translatedText,
                SourceLanguage = request.SourceLanguage ?? "en",
                TargetLanguage = request.TargetLanguage,
                TranslationSuccess = translatedText != request.Text
            };

            return Ok(new ApiResponse<TranslationTestResult>
            {
                Success = true,
                Message = "Translation test completed successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing translation");
            return StatusCode(500, new ApiResponse<TranslationTestResult>
            {
                Success = false,
                Message = "Error testing translation"
            });
        }
    }

    private float CalculateConfidence(string text, string detectedLanguage)
    {
        // Simple confidence calculation based on text characteristics
        if (string.IsNullOrWhiteSpace(text))
            return 0f;

        var confidence = 0.5f; // Base confidence

        // Increase confidence based on script detection
        switch (detectedLanguage)
        {
            case "hi":
            case "sa":
            case "mr":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0900-\u097F]"))
                    confidence += 0.4f;
                break;
            case "ta":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0B80-\u0BFF]"))
                    confidence += 0.4f;
                break;
            case "te":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0C00-\u0C7F]"))
                    confidence += 0.4f;
                break;
            case "bn":
            case "as":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0980-\u09FF]"))
                    confidence += 0.4f;
                break;
            case "gu":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0A80-\u0AFF]"))
                    confidence += 0.4f;
                break;
            case "kn":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0C80-\u0CFF]"))
                    confidence += 0.4f;
                break;
            case "ml":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0D00-\u0D7F]"))
                    confidence += 0.4f;
                break;
            case "pa":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0A00-\u0A7F]"))
                    confidence += 0.4f;
                break;
            case "or":
                if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u0B00-\u0B7F]"))
                    confidence += 0.4f;
                break;
        }

        return Math.Min(1.0f, confidence);
    }

    private string GetDetectionMethod(HttpContext context)
    {
        if (!string.IsNullOrWhiteSpace(context.Request.Query["lang"].FirstOrDefault()))
            return "Query Parameter";
        
        if (!string.IsNullOrWhiteSpace(context.Request.Headers["X-Language"].FirstOrDefault()))
            return "Custom Header";
        
        if (!string.IsNullOrWhiteSpace(context.Session.GetString("Language")))
            return "Session";
        
        if (!string.IsNullOrWhiteSpace(context.Request.Cookies["Language"]))
            return "Cookie";
        
        if (!string.IsNullOrWhiteSpace(context.Request.Headers["Accept-Language"].FirstOrDefault()))
            return "Accept-Language Header";
        
        return "Auto-Detection";
    }
}

// DTOs for language operations
public class LanguageDetectionRequest
{
    public string Text { get; set; } = string.Empty;
}

public class LanguageDetectionResult
{
    public string DetectedLanguage { get; set; } = string.Empty;
    public bool IsSupported { get; set; }
    public float Confidence { get; set; }
    public string FallbackLanguage { get; set; } = string.Empty;
}

public class UserLanguagePreference
{
    public string PreferredLanguage { get; set; } = string.Empty;
    public bool IsSupported { get; set; }
    public string FallbackLanguage { get; set; } = string.Empty;
    public string DetectionMethod { get; set; } = string.Empty;
}

public class SetLanguagePreferenceRequest
{
    public string LanguageCode { get; set; } = string.Empty;
}

public class TranslationTestRequest
{
    public string Text { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
}

public class TranslationTestResult
{
    public string OriginalText { get; set; } = string.Empty;
    public string TranslatedText { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public bool TranslationSuccess { get; set; }
}

