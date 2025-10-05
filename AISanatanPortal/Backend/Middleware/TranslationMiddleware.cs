using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace AISanatanPortal.API.Middleware;

public class TranslationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TranslationMiddleware> _logger;

    public TranslationMiddleware(RequestDelegate next, ILogger<TranslationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow CORS preflight requests to pass through untouched
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Extract language preference from headers or query parameters
        var targetLanguage = ExtractLanguagePreference(context);
        var sourceLanguage = "en"; // Default source language

        // Store language preference in context for later use
        context.Items["TargetLanguage"] = targetLanguage;
        context.Items["SourceLanguage"] = sourceLanguage;

        // Capture the original response stream
        var originalBodyStream = context.Response.Body;

        try
        {
            // Create a new memory stream for the response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continue to the next middleware
            await _next(context);

            // Resolve scoped translation service per-request
            var translationService = context.RequestServices.GetRequiredService<ITranslationService>();

            // Check if translation is needed
            if (await translationService.IsTranslationRequiredAsync(targetLanguage, sourceLanguage) &&
                IsTranslatableResponse(context))
            {
                // Get the response content
                var responseContent = await GetResponseContentAsync(responseBody);
                
                if (!string.IsNullOrEmpty(responseContent))
                {
                    // Translate the response content
                    var translatedContent = await TranslateResponseContentAsync(responseContent, targetLanguage, sourceLanguage, context, translationService);
                    
                    if (translatedContent != responseContent)
                    {
                        // Write the translated content back to the response
                        var translatedBytes = Encoding.UTF8.GetBytes(translatedContent);
                        context.Response.ContentLength = translatedBytes.Length;
                        
                        await originalBodyStream.WriteAsync(translatedBytes, 0, translatedBytes.Length);
                        return;
                    }
                }
            }

            // If no translation was needed or translation failed, copy the original response
            await CopyResponseAsync(responseBody, originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in translation middleware");
            // Copy original response on error
            await CopyResponseAsync(context.Response.Body, originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private string ExtractLanguagePreference(HttpContext context)
    {
        // Priority order: Query parameter > Accept-Language header > Default
        var queryLang = context.Request.Query["lang"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(queryLang))
        {
            return NormalizeLanguageCode(queryLang);
        }

        var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            return ExtractLanguageFromAcceptHeader(acceptLanguage);
        }

        // Check for custom language header
        var customLang = context.Request.Headers["X-Language"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(customLang))
        {
            return NormalizeLanguageCode(customLang);
        }

        return "en"; // Default to English
    }

    private string ExtractLanguageFromAcceptHeader(string acceptLanguage)
    {
        // Parse Accept-Language header (e.g., "en-US,en;q=0.9,hi;q=0.8")
        var languages = acceptLanguage.Split(',')
            .Select(lang => lang.Split(';')[0].Trim())
            .ToList();

        foreach (var lang in languages)
        {
            var normalizedLang = NormalizeLanguageCode(lang);
            if (normalizedLang != "en") // Prefer non-English if available
            {
                return normalizedLang;
            }
        }

        return "en";
    }

    private string NormalizeLanguageCode(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "en";

        // Handle language-region codes (e.g., "en-US" -> "en")
        var normalizedCode = languageCode.Split('-')[0].ToLower();

        // Map common variations
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

    private bool IsTranslatableResponse(HttpContext context)
    {
        // Only translate JSON responses
        var contentType = context.Response.ContentType?.ToLower();
        return contentType?.Contains("application/json") == true &&
               context.Response.StatusCode >= 200 &&
               context.Response.StatusCode < 300;
    }

    private async Task<string> GetResponseContentAsync(Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private async Task<string> TranslateResponseContentAsync(string content, string targetLanguage, string sourceLanguage, HttpContext context, ITranslationService translationService)
    {
        try
        {
            // Try to parse as ApiResponse<T>
            var jsonDocument = JsonDocument.Parse(content);
            var root = jsonDocument.RootElement;

            // Check if it's an ApiResponse structure
            if (root.TryGetProperty("success", out _) || root.TryGetProperty("message", out _))
            {
                // Deserialize as ApiResponse<object>
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null)
                {
                    var translatedResponse = await translationService.TranslateApiResponseAsync(apiResponse, targetLanguage, sourceLanguage);
                    return JsonSerializer.Serialize(translatedResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    });
                }
            }

            // If not ApiResponse, try to translate as generic object
            var translatedContent = await translationService.TranslateObjectAsync(new { Content = content }, targetLanguage, sourceLanguage);
            return JsonSerializer.Serialize(translatedContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to translate response content, returning original");
            return content;
        }
    }

    private async Task CopyResponseAsync(Stream source, Stream destination)
    {
        source.Seek(0, SeekOrigin.Begin);
        await source.CopyToAsync(destination);
    }
}

// Extension method to register the middleware
public static class TranslationMiddlewareExtensions
{
    public static IApplicationBuilder UseTranslation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TranslationMiddleware>();
    }
}

