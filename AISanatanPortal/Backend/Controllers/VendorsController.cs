using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VendorsController> _logger;

    public VendorsController(ApplicationDbContext context, ILogger<VendorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Vendor>>>> GetVendors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] VendorStatus? status = null,
        [FromQuery] string? city = null,
        [FromQuery] string? state = null)
    {
        try
        {
            var query = _context.Vendors.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.Name.Contains(search) || 
                                        v.Email.Contains(search) ||
                                        v.Description!.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(v => v.City!.Contains(city));
            }

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(v => v.State!.Contains(state));
            }

            var totalCount = await query.CountAsync();
            var vendors = await query
                .OrderBy(v => v.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Vendor>
            {
                Items = vendors,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Vendor>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vendors");
            return StatusCode(500, new ApiResponse<PagedResult<Vendor>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Vendor>>> GetVendor(Guid id)
    {
        try
        {
            var vendor = await _context.Vendors
                .Include(v => v.Products)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
            {
                return NotFound(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            return Ok(new ApiResponse<Vendor>
            {
                Success = true,
                Data = vendor
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vendor with ID: {VendorId}", id);
            return StatusCode(500, new ApiResponse<Vendor>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Vendor>>> CreateVendor([FromBody] CreateVendorRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            // Check if vendor already exists
            if (await _context.Vendors.AnyAsync(v => v.Email == request.Email))
            {
                return BadRequest(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Vendor with this email already exists"
                });
            }

            var vendor = new Vendor
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                Status = request.Status,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVendor), new { id = vendor.Id }, new ApiResponse<Vendor>
            {
                Success = true,
                Message = "Vendor created successfully",
                Data = vendor
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vendor");
            return StatusCode(500, new ApiResponse<Vendor>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Vendor>>> UpdateVendor(Guid id, [FromBody] UpdateVendorRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            vendor.Name = request.Name;
            vendor.Email = request.Email;
            vendor.Phone = request.Phone;
            vendor.Address = request.Address;
            vendor.City = request.City;
            vendor.State = request.State;
            vendor.Country = request.Country;
            vendor.PostalCode = request.PostalCode;
            vendor.Description = request.Description;
            vendor.LogoUrl = request.LogoUrl;
            vendor.Status = request.Status;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Vendor>
            {
                Success = true,
                Message = "Vendor updated successfully",
                Data = vendor
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vendor with ID: {VendorId}", id);
            return StatusCode(500, new ApiResponse<Vendor>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVendor(Guid id)
    {
        try
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            // Check if vendor has products
            var hasProducts = await _context.Products.AnyAsync(p => p.VendorId == id && p.IsActive);
            if (hasProducts)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete vendor that has active products"
                });
            }

            vendor.IsActive = false;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Vendor deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vendor with ID: {VendorId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveVendor(Guid id)
    {
        try
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            vendor.Status = VendorStatus.Approved;
            vendor.VerifiedAt = DateTime.UtcNow;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Vendor approved successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving vendor with ID: {VendorId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/suspend")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> SuspendVendor(Guid id)
    {
        try
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            vendor.Status = VendorStatus.Suspended;
            vendor.UpdatedAt = DateTime.UtcNow;
            vendor.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Vendor suspended successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending vendor with ID: {VendorId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Vendor operations
public class CreateVendorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public VendorStatus Status { get; set; } = VendorStatus.Pending;
}

public class UpdateVendorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public VendorStatus Status { get; set; }
}
