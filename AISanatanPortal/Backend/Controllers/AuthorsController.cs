using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Author>>>> GetAuthors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] AuthorType? type = null)
    {
        try
        {
            var query = _context.Authors.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Name.Contains(search) || 
                                        a.SanskritName!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(a => a.Type == type.Value);
            }

            var totalCount = await query.CountAsync();
            var authors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Author>
            {
                Items = authors,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Author>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors");
            return StatusCode(500, new ApiResponse<PagedResult<Author>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Author>>> GetAuthor(Guid id)
    {
        try
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound(new ApiResponse<Author>
                {
                    Success = false,
                    Message = "Author not found"
                });
            }

            return Ok(new ApiResponse<Author>
            {
                Success = true,
                Data = author
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving author with ID: {AuthorId}", id);
            return StatusCode(500, new ApiResponse<Author>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Author>>> CreateAuthor([FromBody] CreateAuthorRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Author>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var author = new Author
            {
                Name = request.Name,
                SanskritName = request.SanskritName,
                Biography = request.Biography,
                BirthDate = request.BirthDate,
                DeathDate = request.DeathDate,
                BirthPlace = request.BirthPlace,
                ProfileImageUrl = request.ProfileImageUrl,
                Type = request.Type,
                IsVerified = request.IsVerified,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, new ApiResponse<Author>
            {
                Success = true,
                Message = "Author created successfully",
                Data = author
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author");
            return StatusCode(500, new ApiResponse<Author>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Author>>> UpdateAuthor(Guid id, [FromBody] UpdateAuthorRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Author>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new ApiResponse<Author>
                {
                    Success = false,
                    Message = "Author not found"
                });
            }

            author.Name = request.Name;
            author.SanskritName = request.SanskritName;
            author.Biography = request.Biography;
            author.BirthDate = request.BirthDate;
            author.DeathDate = request.DeathDate;
            author.BirthPlace = request.BirthPlace;
            author.ProfileImageUrl = request.ProfileImageUrl;
            author.Type = request.Type;
            author.IsVerified = request.IsVerified;
            author.UpdatedAt = DateTime.UtcNow;
            author.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Author>
            {
                Success = true,
                Message = "Author updated successfully",
                Data = author
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author with ID: {AuthorId}", id);
            return StatusCode(500, new ApiResponse<Author>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAuthor(Guid id)
    {
        try
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Author not found"
                });
            }

            author.IsActive = false;
            author.UpdatedAt = DateTime.UtcNow;
            author.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Author deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID: {AuthorId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Author operations
public class CreateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? ProfileImageUrl { get; set; }
    public AuthorType Type { get; set; } = AuthorType.Contemporary;
    public bool IsVerified { get; set; } = false;
}

public class UpdateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? ProfileImageUrl { get; set; }
    public AuthorType Type { get; set; }
    public bool IsVerified { get; set; }
}
