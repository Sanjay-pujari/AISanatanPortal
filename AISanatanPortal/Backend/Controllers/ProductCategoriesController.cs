using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductCategoriesController> _logger;

    public ProductCategoriesController(ApplicationDbContext context, ILogger<ProductCategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductCategory>>>> GetProductCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? parentCategoryId = null)
    {
        try
        {
            var query = _context.ProductCategories
                .Include(pc => pc.ParentCategory)
                .Include(pc => pc.SubCategories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(pc => pc.Name.Contains(search) || 
                                         pc.SanskritName!.Contains(search) ||
                                         pc.Description!.Contains(search));
            }

            if (parentCategoryId.HasValue)
            {
                query = query.Where(pc => pc.ParentCategoryId == parentCategoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var categories = await query
                .OrderBy(pc => pc.DisplayOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<ProductCategory>
            {
                Items = categories,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<ProductCategory>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product categories");
            return StatusCode(500, new ApiResponse<PagedResult<ProductCategory>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryTreeDto>>>> GetProductCategoriesTree()
    {
        try
        {
            var categories = await _context.ProductCategories
                .Where(pc => pc.IsActive)
                .OrderBy(pc => pc.DisplayOrder)
                .ToListAsync();

            var categoryTree = BuildCategoryTree(categories);

            return Ok(new ApiResponse<List<ProductCategoryTreeDto>>
            {
                Success = true,
                Data = categoryTree
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product categories tree");
            return StatusCode(500, new ApiResponse<List<ProductCategoryTreeDto>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductCategory>>> GetProductCategory(Guid id)
    {
        try
        {
            var category = await _context.ProductCategories
                .Include(pc => pc.ParentCategory)
                .Include(pc => pc.SubCategories)
                .Include(pc => pc.Products)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<ProductCategory>
                {
                    Success = false,
                    Message = "Product category not found"
                });
            }

            return Ok(new ApiResponse<ProductCategory>
            {
                Success = true,
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<ProductCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ProductCategory>>> CreateProductCategory([FromBody] CreateProductCategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ProductCategory>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = new ProductCategory
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

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductCategory), new { id = category.Id }, new ApiResponse<ProductCategory>
            {
                Success = true,
                Message = "Product category created successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product category");
            return StatusCode(500, new ApiResponse<ProductCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ProductCategory>>> UpdateProductCategory(Guid id, [FromBody] UpdateProductCategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ProductCategory>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<ProductCategory>
                {
                    Success = false,
                    Message = "Product category not found"
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

            return Ok(new ApiResponse<ProductCategory>
            {
                Success = true,
                Message = "Product category updated successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<ProductCategory>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProductCategory(Guid id)
    {
        try
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Product category not found"
                });
            }

            // Check if category has products or subcategories
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id && p.IsActive);
            var hasSubCategories = await _context.ProductCategories.AnyAsync(pc => pc.ParentCategoryId == id && pc.IsActive);

            if (hasProducts || hasSubCategories)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete category that has products or subcategories"
                });
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Product category deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product category with ID: {CategoryId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    private List<ProductCategoryTreeDto> BuildCategoryTree(List<ProductCategory> categories)
    {
        var categoryDict = categories.ToDictionary(c => c.Id, c => new ProductCategoryTreeDto
        {
            Id = c.Id,
            Name = c.Name,
            SanskritName = c.SanskritName,
            Description = c.Description,
            ParentCategoryId = c.ParentCategoryId,
            IconUrl = c.IconUrl,
            DisplayOrder = c.DisplayOrder,
            Children = new List<ProductCategoryTreeDto>()
        });

        var rootCategories = new List<ProductCategoryTreeDto>();

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

// DTOs for ProductCategory operations
public class CreateProductCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateProductCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProductCategoryTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public List<ProductCategoryTreeDto> Children { get; set; } = new List<ProductCategoryTreeDto>();
}
