using AISanatanPortal.API.Models;
using AISanatanPortal.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AISanatanPortal.API.Services;

public interface IDataValidationService
{
    Task<bool> IsVedaDuplicateAsync(string name, string sanskritName);
    Task<bool> IsPuranaDuplicateAsync(string name, string sanskritName);
    Task<bool> IsKavyaDuplicateAsync(string name, string author);
    Task<bool> IsTempleDuplicateAsync(string name, decimal latitude, decimal longitude);
    Task<bool> IsMythologicalPlaceDuplicateAsync(string name, decimal latitude, decimal longitude);
    
    Task<bool> ValidateVedaDataAsync(Veda veda);
    Task<bool> ValidatePuranaDataAsync(Purana purana);
    Task<bool> ValidateKavyaDataAsync(Kavya kavya);
    Task<bool> ValidateTempleDataAsync(Temple temple);
    Task<bool> ValidateMythologicalPlaceDataAsync(MythologicalPlace place);
    
    Task<string> CleanAndNormalizeTextAsync(string text);
    Task<string> ExtractSanskritNameAsync(string content);
    Task<string> ExtractDescriptionAsync(string content);
}

public class DataValidationService : IDataValidationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataValidationService> _logger;

    public DataValidationService(ApplicationDbContext context, ILogger<DataValidationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsVedaDuplicateAsync(string name, string sanskritName)
    {
        try
        {
            var normalizedName = name.ToLower().Trim();
            var normalizedSanskrit = sanskritName?.ToLower().Trim() ?? string.Empty;
            
            return await _context.Vedas.AnyAsync(v => 
                v.Name.ToLower() == normalizedName || 
                (v.SanskritName != null && v.SanskritName.ToLower() == normalizedSanskrit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Veda duplicate for name: {Name}", name);
            return true; // Assume duplicate to be safe
        }
    }

    public async Task<bool> IsPuranaDuplicateAsync(string name, string sanskritName)
    {
        try
        {
            var normalizedName = name.ToLower().Trim();
            var normalizedSanskrit = sanskritName?.ToLower().Trim() ?? string.Empty;
            
            return await _context.Puranas.AnyAsync(p => 
                p.Name.ToLower() == normalizedName || 
                (p.SanskritName != null && p.SanskritName.ToLower() == normalizedSanskrit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Purana duplicate for name: {Name}", name);
            return true;
        }
    }

    public async Task<bool> IsKavyaDuplicateAsync(string name, string author)
    {
        try
        {
            var normalizedName = name.ToLower().Trim();
            var normalizedAuthor = author.ToLower().Trim();
            
            return await _context.Kavyas.AnyAsync(k => 
                k.Name.ToLower() == normalizedName && k.Author.ToLower() == normalizedAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Kavya duplicate for name: {Name}, author: {Author}", name, author);
            return true;
        }
    }

    public async Task<bool> IsTempleDuplicateAsync(string name, decimal latitude, decimal longitude)
    {
        try
        {
            var normalizedName = name.ToLower().Trim();
            const decimal tolerance = 0.001m; // ~100 meters tolerance
            
            return await _context.Temples.AnyAsync(t => 
                t.Name.ToLower() == normalizedName || 
                (Math.Abs(t.Latitude - latitude) < tolerance && Math.Abs(t.Longitude - longitude) < tolerance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Temple duplicate for name: {Name}", name);
            return true;
        }
    }

    public async Task<bool> IsMythologicalPlaceDuplicateAsync(string name, decimal latitude, decimal longitude)
    {
        try
        {
            var normalizedName = name.ToLower().Trim();
            const decimal tolerance = 0.001m;
            
            return await _context.MythologicalPlaces.AnyAsync(m => 
                m.Name.ToLower() == normalizedName || 
                (Math.Abs(m.Latitude - latitude) < tolerance && Math.Abs(m.Longitude - longitude) < tolerance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking MythologicalPlace duplicate for name: {Name}", name);
            return true;
        }
    }

    public async Task<bool> ValidateVedaDataAsync(Veda veda)
    {
        if (string.IsNullOrWhiteSpace(veda.Name))
            return false;
            
        if (veda.ChapterCount < 0 || veda.VerseCount < 0)
            return false;
            
        return true;
    }

    public async Task<bool> ValidatePuranaDataAsync(Purana purana)
    {
        if (string.IsNullOrWhiteSpace(purana.Name))
            return false;
            
        if (purana.ChapterCount < 0 || purana.StoryCount < 0)
            return false;
            
        return true;
    }

    public async Task<bool> ValidateKavyaDataAsync(Kavya kavya)
    {
        if (string.IsNullOrWhiteSpace(kavya.Name) || string.IsNullOrWhiteSpace(kavya.Author))
            return false;
            
        if (kavya.ChapterCount < 0)
            return false;
            
        return true;
    }

    public async Task<bool> ValidateTempleDataAsync(Temple temple)
    {
        if (string.IsNullOrWhiteSpace(temple.Name))
            return false;
            
        if (temple.Latitude < -90 || temple.Latitude > 90)
            return false;
            
        if (temple.Longitude < -180 || temple.Longitude > 180)
            return false;
            
        return true;
    }

    public async Task<bool> ValidateMythologicalPlaceDataAsync(MythologicalPlace place)
    {
        if (string.IsNullOrWhiteSpace(place.Name))
            return false;
            
        if (place.Latitude < -90 || place.Latitude > 90)
            return false;
            
        if (place.Longitude < -180 || place.Longitude > 180)
            return false;
            
        return true;
    }

    public async Task<string> CleanAndNormalizeTextAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
            
        // Remove extra whitespace and normalize
        var cleaned = Regex.Replace(text, @"\s+", " ").Trim();
        
        // Remove HTML tags if any
        cleaned = Regex.Replace(cleaned, @"<[^>]*>", "");
        
        // Remove special characters but keep basic punctuation
        cleaned = Regex.Replace(cleaned, @"[^\w\s.,!?;:()-]", "");
        
        return cleaned;
    }

    public async Task<string> ExtractSanskritNameAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;
            
        // Look for Sanskrit text patterns (Devanagari script)
        var sanskritPattern = @"[\u0900-\u097F]+";
        var matches = Regex.Matches(content, sanskritPattern);
        
        if (matches.Count > 0)
        {
            return matches[0].Value.Trim();
        }
        
        return string.Empty;
    }

    public async Task<string> ExtractDescriptionAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;
            
        // Extract first meaningful paragraph
        var sentences = content.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var description = string.Empty;
        
        foreach (var sentence in sentences)
        {
            var trimmed = sentence.Trim();
            if (trimmed.Length > 50 && trimmed.Length < 500)
            {
                description = trimmed;
                break;
            }
        }
        
        return description.Length > 0 ? description : content.Substring(0, Math.Min(200, content.Length));
    }
}
