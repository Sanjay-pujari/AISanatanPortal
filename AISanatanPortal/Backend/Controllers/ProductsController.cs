using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Product>>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? vendorId = null,
        [FromQuery] ProductStatus? status = null,
        [FromQuery] bool? isFeatured = null)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                        p.SanskritName!.Contains(search) ||
                                        p.Description!.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (vendorId.HasValue)
            {
                query = query.Where(p => p.VendorId == vendorId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(p => p.IsFeatured == isFeatured.Value);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Product>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, new ApiResponse<PagedResult<Product>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Product>>> GetProduct(Guid id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new ApiResponse<Product>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<Product>
            {
                Success = true,
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
            return StatusCode(500, new ApiResponse<Product>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Vendor")]
    public async Task<ActionResult<ApiResponse<Product>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Product>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var product = new Product
            {
                Name = request.Name,
                SanskritName = request.SanskritName,
                Description = request.Description,
                CategoryId = request.CategoryId,
                VendorId = request.VendorId,
                SKU = request.SKU,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                StockQuantity = request.StockQuantity,
                IsInStock = request.IsInStock,
                Status = request.Status,
                PrimaryImageUrl = request.PrimaryImageUrl,
                ImageUrls = request.ImageUrls,
                Weight = request.Weight,
                Dimensions = request.Dimensions,
                Material = request.Material,
                Color = request.Color,
                IsFeatured = request.IsFeatured,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ApiResponse<Product>
            {
                Success = true,
                Message = "Product created successfully",
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new ApiResponse<Product>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin,Vendor")]
    public async Task<ActionResult<ApiResponse<Product>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Product>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<Product>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            product.Name = request.Name;
            product.SanskritName = request.SanskritName;
            product.Description = request.Description;
            product.CategoryId = request.CategoryId;
            product.VendorId = request.VendorId;
            product.SKU = request.SKU;
            product.Price = request.Price;
            product.DiscountPrice = request.DiscountPrice;
            product.StockQuantity = request.StockQuantity;
            product.IsInStock = request.IsInStock;
            product.Status = request.Status;
            product.PrimaryImageUrl = request.PrimaryImageUrl;
            product.ImageUrls = request.ImageUrls;
            product.Weight = request.Weight;
            product.Dimensions = request.Dimensions;
            product.Material = request.Material;
            product.Color = request.Color;
            product.IsFeatured = request.IsFeatured;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Product>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
            return StatusCode(500, new ApiResponse<Product>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Product deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/update-stock")]
    [Authorize(Roles = "Admin,SuperAdmin,Vendor")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            product.StockQuantity = request.StockQuantity;
            product.IsInStock = request.StockQuantity > 0;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Stock updated successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product with ID: {ProductId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Product operations
public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VendorId { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; } = 0;
    public bool IsInStock { get; set; } = true;
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    public string? PrimaryImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public decimal Weight { get; set; } = 0;
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? Color { get; set; }
    public bool IsFeatured { get; set; } = false;
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VendorId { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public ProductStatus Status { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public decimal Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? Color { get; set; }
    public bool IsFeatured { get; set; }
}

public class UpdateStockRequest
{
    public int StockQuantity { get; set; }
}
