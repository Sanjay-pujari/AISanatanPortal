using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BookCategoriesController> _logger;

    public BookCategoriesController(ApplicationDbContext context, ILogger<BookCategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BookCategory>>>> GetBookCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? parentCategoryId = null)
    {
        try
        {
            var query = _context.BookCategories
                .Include(bc => bc.ParentCategory)
                .Include(bc => bc.SubCategories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(bc => bc.Name.Contains(search) || 
                                         bc.SanskritName!.Contains(search) ||
                                         bc.Description!.Contains(search));
            }

            if (parentCategoryId.HasValue)
            {
                query = query.Where(bc => bc.ParentCategoryId == parentCategoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var categories = await query
                .OrderBy(bc => bc.DisplayOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<BookCategory>
            {
                Items = categories,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<BookCategory>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book categories");
            return StatusCode(500, new ApiResponse<PagedResult<BookCategory>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<BookCategoryTreeDto>>>> GetBookCategoriesTree()
    {
        try
        {
            var categories = await _context.BookCategories
                .Where(bc => bc.IsActive)
                .OrderBy(bc => bc.DisplayOrder)
                .ToListAsync();

            var categoryTree = BuildCategoryTree(categories);

            return Ok(new ApiResponse<List<BookCategoryTreeDto>>
            {
                Success = true,
                Data = categoryTree
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book categories tree");
            return StatusCode(500, new ApiResponse<List<BookCategoryTreeDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookCategory>>> GetBookCategory(Guid id)
    {
        try
        {
            var category = await _context.BookCategories
                .Include(bc => bc.ParentCategory)
                .Include(bc => bc.SubCategories)
                .Include(bc => bc.Books)
                .FirstOrDefaultAsync(bc => bc.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<BookCategory>
                {
                    Success = false,
                    Message = "Book category not found"
                });
            }

            return Ok(new ApiResponse<BookCategory>
            {
                Success = true,
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<BookCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<BookCategory>>> CreateBookCategory([FromBody] CreateBookCategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<BookCategory>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = new BookCategory
            {
                Name = request.Name,
                SanskritName = request.SanskritName,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId,
                IconUrl = request.IconUrl,
                DisplayOrder = request.DisplayOrder,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.BookCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookCategory), new { id = category.Id }, new ApiResponse<BookCategory>
            {
                Success = true,
                Message = "Book category created successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book category");
            return StatusCode(500, new ApiResponse<BookCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<BookCategory>>> UpdateBookCategory(Guid id, [FromBody] UpdateBookCategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<BookCategory>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = await _context.BookCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<BookCategory>
                {
                    Success = false,
                    Message = "Book category not found"
                });
            }

            category.Name = request.Name;
            category.SanskritName = request.SanskritName;
            category.Description = request.Description;
            category.ParentCategoryId = request.ParentCategoryId;
            category.IconUrl = request.IconUrl;
            category.DisplayOrder = request.DisplayOrder;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<BookCategory>
            {
                Success = true,
                Message = "Book category updated successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<BookCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBookCategory(Guid id)
    {
        try
        {
            var category = await _context.BookCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Book category not found"
                });
            }

            // Check if category has books or subcategories
            var hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == id && b.IsActive);
            var hasSubCategories = await _context.BookCategories.AnyAsync(bc => bc.ParentCategoryId == id && bc.IsActive);

            if (hasBooks || hasSubCategories)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete category that has books or subcategories"
                });
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Book category deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    private List<BookCategoryTreeDto> BuildCategoryTree(List<BookCategory> categories)
    {
        var categoryDict = categories.ToDictionary(c => c.Id, c => new BookCategoryTreeDto
        {
            Id = c.Id,
            Name = c.Name,
            SanskritName = c.SanskritName,
            Description = c.Description,
            ParentCategoryId = c.ParentCategoryId,
            IconUrl = c.IconUrl,
            DisplayOrder = c.DisplayOrder,
            Children = new List<BookCategoryTreeDto>()
        });

        var rootCategories = new List<BookCategoryTreeDto>();

        foreach (var category in categories)
        {
            if (category.ParentCategoryId == null)
            {
                rootCategories.Add(categoryDict[category.Id]);
            }
            else if (categoryDict.ContainsKey(category.ParentCategoryId.Value))
            {
                categoryDict[category.ParentCategoryId.Value].Children.Add(categoryDict[category.Id]);
            }
        }

        return rootCategories.OrderBy(c => c.DisplayOrder).ToList();
    }
}

// DTOs for BookCategory operations
public class CreateBookCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateBookCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class BookCategoryTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public List<BookCategoryTreeDto> Children { get; set; } = new List<BookCategoryTreeDto>();
}
