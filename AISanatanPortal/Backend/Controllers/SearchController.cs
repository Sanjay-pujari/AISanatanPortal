using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ApplicationDbContext context, ILogger<SearchController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public class SearchResultItem
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Book, Author, Category, Product, Event, Temple, User
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Snippet { get; set; }
        public string? Route { get; set; }
    }

    public class SearchResponse
    {
        public List<SearchResultItem> Items { get; set; } = new();
        public int TotalCount => Items.Count;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SearchResponse>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(new ApiResponse<SearchResponse> { Success = true, Data = new SearchResponse() });
        }

        try
        {
            var term = q.Trim();
            var termLower = term.ToLower();

            var books = await _context.Books
                .Where(b => (
                    b.Title.ToLower().Contains(termLower) ||
                    ((b.SanskritTitle ?? "").ToLower().Contains(termLower)) ||
                    ((b.Description ?? "").ToLower().Contains(termLower))
                ) && b.IsActive)
                .Select(b => new SearchResultItem
                {
                    Id = b.Id.ToString(),
                    Type = "Book",
                    Title = b.Title,
                    Subtitle = b.SanskritTitle,
                    Snippet = b.Description,
                    Route = $"/admin/books"
                })
                .Take(20)
                .ToListAsync();

            var authors = await _context.Authors
                .Where(a => (
                    a.Name.ToLower().Contains(termLower) ||
                    ((a.SanskritName ?? "").ToLower().Contains(termLower)) ||
                    ((a.Biography ?? "").ToLower().Contains(termLower))
                ) && a.IsActive)
                .Select(a => new SearchResultItem
                {
                    Id = a.Id.ToString(),
                    Type = "Author",
                    Title = a.Name,
                    Subtitle = a.SanskritName,
                    Snippet = a.Biography,
                    Route = "/admin/authors"
                })
                .Take(20)
                .ToListAsync();

            var categories = await _context.BookCategories
                .Where(c => (
                    c.Name.ToLower().Contains(termLower) ||
                    ((c.SanskritName ?? "").ToLower().Contains(termLower)) ||
                    ((c.Description ?? "").ToLower().Contains(termLower))
                ) && c.IsActive)
                .Select(c => new SearchResultItem
                {
                    Id = c.Id.ToString(),
                    Type = "Book Category",
                    Title = c.Name,
                    Subtitle = c.SanskritName,
                    Snippet = c.Description,
                    Route = "/admin/book-categories"
                })
                .Take(20)
                .ToListAsync();

            var products = await _context.Products
                .Where(p => (
                    p.Name.ToLower().Contains(termLower) ||
                    ((p.Description ?? "").ToLower().Contains(termLower))
                ) && p.IsActive)
                .Select(p => new SearchResultItem
                {
                    Id = p.Id.ToString(),
                    Type = "Product",
                    Title = p.Name,
                    Snippet = p.Description,
                    Route = "/admin/products"
                })
                .Take(20)
                .ToListAsync();

            var events = await _context.Events
                .Where(e => (
                    e.Title.ToLower().Contains(termLower) ||
                    ((e.Description ?? "").ToLower().Contains(termLower)) ||
                    ((e.Tags ?? "").ToLower().Contains(termLower))
                ) && e.IsActive)
                .Select(e => new SearchResultItem
                {
                    Id = e.Id.ToString(),
                    Type = "Event",
                    Title = e.Title,
                    Subtitle = e.Category.ToString(),
                    Snippet = e.Description,
                    Route = "/admin/events"
                })
                .Take(20)
                .ToListAsync();

            var temples = await _context.Temples
                .Where(t => (
                    t.Name.ToLower().Contains(termLower) ||
                    ((t.Description ?? "").ToLower().Contains(termLower)) ||
                    ((t.Significance ?? "").ToLower().Contains(termLower)) ||
                    ((t.History ?? "").ToLower().Contains(termLower))
                ) && t.IsActive)
                .Select(t => new SearchResultItem
                {
                    Id = t.Id.ToString(),
                    Type = "Temple",
                    Title = t.Name,
                    Snippet = t.Description,
                    Route = "/admin/temples"
                })
                .Take(20)
                .ToListAsync();

            var mythPlaces = await _context.MythologicalPlaces
                .Where(m => (
                    m.Name.ToLower().Contains(termLower) ||
                    ((m.Description ?? "").ToLower().Contains(termLower)) ||
                    ((m.MythologicalSignificance ?? "").ToLower().Contains(termLower)) ||
                    ((m.RelatedTexts ?? "").ToLower().Contains(termLower))
                ) && m.IsActive)
                .Select(m => new SearchResultItem
                {
                    Id = m.Id.ToString(),
                    Type = "Mythological Place",
                    Title = m.Name,
                    Snippet = m.Description,
                    Route = "/admin/places-temples"
                })
                .Take(20)
                .ToListAsync();

            var vedas = await _context.Vedas
                .Where(v => (
                    v.Name.ToLower().Contains(termLower) ||
                    ((v.SanskritName ?? "").ToLower().Contains(termLower)) ||
                    ((v.Description ?? "").ToLower().Contains(termLower))
                ) && v.IsActive)
                .Select(v => new SearchResultItem
                {
                    Id = v.Id.ToString(),
                    Type = "Veda",
                    Title = v.Name,
                    Subtitle = v.SanskritName,
                    Snippet = v.Description,
                    Route = "/vedas"
                })
                .Take(20)
                .ToListAsync();

            var puranas = await _context.Puranas
                .Where(p => (
                    p.Name.ToLower().Contains(termLower) ||
                    ((p.SanskritName ?? "").ToLower().Contains(termLower)) ||
                    ((p.Description ?? "").ToLower().Contains(termLower))
                ) && p.IsActive)
                .Select(p => new SearchResultItem
                {
                    Id = p.Id.ToString(),
                    Type = "Purana",
                    Title = p.Name,
                    Subtitle = p.SanskritName,
                    Snippet = p.Description,
                    Route = "/puranas"
                })
                .Take(20)
                .ToListAsync();

            var kavyas = await _context.Kavyas
                .Where(k => (
                    k.Name.ToLower().Contains(termLower) ||
                    ((k.SanskritName ?? "").ToLower().Contains(termLower)) ||
                    ((k.Description ?? "").ToLower().Contains(termLower)) ||
                    k.Author.ToLower().Contains(termLower)
                ) && k.IsActive)
                .Select(k => new SearchResultItem
                {
                    Id = k.Id.ToString(),
                    Type = "Kavya",
                    Title = k.Name,
                    Subtitle = k.Author,
                    Snippet = k.Description,
                    Route = "/kavyas"
                })
                .Take(20)
                .ToListAsync();

            var results = new SearchResponse();
            results.Items.AddRange(books);
            results.Items.AddRange(authors);
            results.Items.AddRange(categories);
            results.Items.AddRange(products);
            results.Items.AddRange(events);
            results.Items.AddRange(temples);
            results.Items.AddRange(mythPlaces);
            results.Items.AddRange(vedas);
            results.Items.AddRange(puranas);
            results.Items.AddRange(kavyas);

            return Ok(new ApiResponse<SearchResponse> { Success = true, Data = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing global search");
            return StatusCode(500, new ApiResponse<SearchResponse> { Success = false, Message = "An internal server error occurred" });
        }
    }
}


