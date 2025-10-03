using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(ApplicationDbContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Book>>>> GetBooks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] BookLanguage? language = null)
    {
        try
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || 
                                        b.SanskritTitle!.Contains(search) ||
                                        b.Description!.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            if (language.HasValue)
            {
                query = query.Where(b => b.Language == language.Value);
            }

            var totalCount = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Book>
            {
                Items = books,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Book>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books");
            return StatusCode(500, new ApiResponse<PagedResult<Book>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Book>>> GetBook(Guid id)
    {
        try
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Book not found"
                });
            }

            return Ok(new ApiResponse<Book>
            {
                Success = true,
                Data = book
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book with ID: {BookId}", id);
            return StatusCode(500, new ApiResponse<Book>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Author")]
    public async Task<ActionResult<ApiResponse<Book>>> CreateBook([FromBody] CreateBookRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var book = new Book
            {
                Title = request.Title,
                SanskritTitle = request.SanskritTitle,
                AuthorId = request.AuthorId,
                CategoryId = request.CategoryId,
                ISBN = request.ISBN,
                Description = request.Description,
                Summary = request.Summary,
                Content = request.Content,
                PageCount = request.PageCount,
                Language = request.Language,
                Format = request.Format,
                Price = request.Price,
                IsFree = request.IsFree,
                CoverImageUrl = request.CoverImageUrl,
                AudioUrl = request.AudioUrl,
                PublishedDate = request.PublishedDate,
                IsFeatured = request.IsFeatured,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new ApiResponse<Book>
            {
                Success = true,
                Message = "Book created successfully",
                Data = book
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book");
            return StatusCode(500, new ApiResponse<Book>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin,Author")]
    public async Task<ActionResult<ApiResponse<Book>>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Book not found"
                });
            }

            book.Title = request.Title;
            book.SanskritTitle = request.SanskritTitle;
            book.AuthorId = request.AuthorId;
            book.CategoryId = request.CategoryId;
            book.ISBN = request.ISBN;
            book.Description = request.Description;
            book.Summary = request.Summary;
            book.Content = request.Content;
            book.PageCount = request.PageCount;
            book.Language = request.Language;
            book.Format = request.Format;
            book.Price = request.Price;
            book.IsFree = request.IsFree;
            book.CoverImageUrl = request.CoverImageUrl;
            book.AudioUrl = request.AudioUrl;
            book.PublishedDate = request.PublishedDate;
            book.IsFeatured = request.IsFeatured;
            book.UpdatedAt = DateTime.UtcNow;
            book.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Book>
            {
                Success = true,
                Message = "Book updated successfully",
                Data = book
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book with ID: {BookId}", id);
            return StatusCode(500, new ApiResponse<Book>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBook(Guid id)
    {
        try
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Book not found"
                });
            }

            book.IsActive = false;
            book.UpdatedAt = DateTime.UtcNow;
            book.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Book deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book with ID: {BookId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Book operations
public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? SanskritTitle { get; set; }
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public BookLanguage Language { get; set; } = BookLanguage.English;
    public BookFormat Format { get; set; } = BookFormat.Digital;
    public decimal Price { get; set; }
    public bool IsFree { get; set; } = true;
    public string? CoverImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public DateTime? PublishedDate { get; set; }
    public bool IsFeatured { get; set; } = false;
}

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? SanskritTitle { get; set; }
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public BookLanguage Language { get; set; }
    public BookFormat Format { get; set; }
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public DateTime? PublishedDate { get; set; }
    public bool IsFeatured { get; set; }
}
