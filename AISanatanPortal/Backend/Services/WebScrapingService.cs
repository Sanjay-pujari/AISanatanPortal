using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AISanatanPortal.API.Services;

public interface IWebScrapingService
{
    Task<string> SearchWikipediaAsync(string query);
    Task<string> SearchGoogleAsync(string query);
    Task<Dictionary<string, string>> ExtractStructuredDataAsync(string url);
    Task<List<string>> GetSearchResultsAsync(string query, int maxResults = 10);
    Task<string> GetPageContentAsync(string url);
}

public class WebScrapingService : IWebScrapingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebScrapingService> _logger;

    public WebScrapingService(HttpClient httpClient, ILogger<WebScrapingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Set user agent to avoid blocking
        _httpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    }

    public async Task<string> SearchWikipediaAsync(string query)
    {
        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{encodedQuery}";
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                
                if (jsonDoc.RootElement.TryGetProperty("extract", out var extract))
                {
                    return extract.GetString() ?? string.Empty;
                }
            }
            
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Wikipedia for query: {Query}", query);
            return string.Empty;
        }
    }

    public async Task<string> SearchGoogleAsync(string query)
    {
        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://www.google.com/search?q={encodedQuery}";
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return ExtractTextFromHtml(content);
            }
            
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Google for query: {Query}", query);
            return string.Empty;
        }
    }

    public async Task<Dictionary<string, string>> ExtractStructuredDataAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                
                var structuredData = new Dictionary<string, string>();
                
                // Extract title
                var titleNode = doc.DocumentNode.SelectSingleNode("//title");
                if (titleNode != null)
                {
                    structuredData["title"] = titleNode.InnerText.Trim();
                }
                
                // Extract meta description
                var metaDescNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                if (metaDescNode != null)
                {
                    structuredData["description"] = metaDescNode.GetAttributeValue("content", "").Trim();
                }
                
                // Extract main content
                var contentNodes = doc.DocumentNode.SelectNodes("//p");
                if (contentNodes != null)
                {
                    var contentText = string.Join(" ", contentNodes.Select(n => n.InnerText.Trim()));
                    structuredData["content"] = contentText;
                }
                
                return structuredData;
            }
            
            return new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting structured data from URL: {Url}", url);
            return new Dictionary<string, string>();
        }
    }

    public async Task<List<string>> GetSearchResultsAsync(string query, int maxResults = 10)
    {
        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://www.google.com/search?q={encodedQuery}";
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                
                var urls = new List<string>();
                var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
                
                if (linkNodes != null)
                {
                    foreach (var link in linkNodes.Take(maxResults))
                    {
                        var href = link.GetAttributeValue("href", "");
                        if (href.StartsWith("http") && !href.Contains("google.com"))
                        {
                            urls.Add(href);
                        }
                    }
                }
                
                return urls.Distinct().ToList();
            }
            
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search results for query: {Query}", query);
            return new List<string>();
        }
    }

    public async Task<string> GetPageContentAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return ExtractTextFromHtml(content);
            }
            
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting page content from URL: {Url}", url);
            return string.Empty;
        }
    }

    private string ExtractTextFromHtml(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            // Remove script and style elements
            var scriptNodes = doc.DocumentNode.SelectNodes("//script");
            if (scriptNodes != null)
            {
                foreach (var node in scriptNodes)
                {
                    node.Remove();
                }
            }
            
            var styleNodes = doc.DocumentNode.SelectNodes("//style");
            if (styleNodes != null)
            {
                foreach (var node in styleNodes)
                {
                    node.Remove();
                }
            }
            
            return doc.DocumentNode.InnerText;
        }
        catch
        {
            return html;
        }
    }
}
