using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplesController> _logger;

    public TemplesController(ApplicationDbContext context, ILogger<TemplesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Temple>>>> GetTemples(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] TempleType? type = null,
        [FromQuery] string? city = null,
        [FromQuery] string? state = null,
        [FromQuery] string? country = null,
        [FromQuery] decimal? minLatitude = null,
        [FromQuery] decimal? maxLatitude = null,
        [FromQuery] decimal? minLongitude = null,
        [FromQuery] decimal? maxLongitude = null)
    {
        try
        {
            var query = _context.Temples
                .Include(t => t.Images)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Name.Contains(search) || 
                                        t.MainDeity!.Contains(search) ||
                                        t.Description!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(t => t.City!.Contains(city));
            }

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(t => t.State!.Contains(state));
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(t => t.Country!.Contains(country));
            }

            // Location-based filtering
            if (minLatitude.HasValue)
                query = query.Where(t => t.Latitude >= minLatitude.Value);
            if (maxLatitude.HasValue)
                query = query.Where(t => t.Latitude <= maxLatitude.Value);
            if (minLongitude.HasValue)
                query = query.Where(t => t.Longitude >= minLongitude.Value);
            if (maxLongitude.HasValue)
                query = query.Where(t => t.Longitude <= maxLongitude.Value);

            var totalCount = await query.CountAsync();
            var temples = await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Temple>
            {
                Items = temples,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Temple>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving temples");
            return StatusCode(500, new ApiResponse<PagedResult<Temple>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<ApiResponse<List<Temple>>>> GetNearbyTemples(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        [FromQuery] decimal radiusKm = 50)
    {
        try
        {
            // Simple distance calculation (for production, consider using proper geographic calculations)
            var temples = await _context.Temples
                .Where(t => t.IsActive)
                .ToListAsync();

            var nearbyTemples = temples
                .Where(t => CalculateDistance(latitude, longitude, t.Latitude, t.Longitude) <= radiusKm)
                .OrderBy(t => CalculateDistance(latitude, longitude, t.Latitude, t.Longitude))
                .Take(20)
                .ToList();

            return Ok(new ApiResponse<List<Temple>>
            {
                Success = true,
                Data = nearbyTemples
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nearby temples");
            return StatusCode(500, new ApiResponse<List<Temple>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Temple>>> GetTemple(Guid id)
    {
        try
        {
            var temple = await _context.Temples
                .Include(t => t.Images)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (temple == null)
            {
                return NotFound(new ApiResponse<Temple>
                {
                    Success = false,
                    Message = "Temple not found"
                });
            }

            return Ok(new ApiResponse<Temple>
            {
                Success = true,
                Data = temple
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving temple with ID: {TempleId}", id);
            return StatusCode(500, new ApiResponse<Temple>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Temple>>> CreateTemple([FromBody] CreateTempleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Temple>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var temple = new Temple
            {
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                Type = request.Type,
                MainDeity = request.MainDeity,
                Significance = request.Significance,
                History = request.History,
                VisitingHours = request.VisitingHours,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                Website = request.Website,
                IsVerified = request.IsVerified,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Temples.Add(temple);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemple), new { id = temple.Id }, new ApiResponse<Temple>
            {
                Success = true,
                Message = "Temple created successfully",
                Data = temple
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating temple");
            return StatusCode(500, new ApiResponse<Temple>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Temple>>> UpdateTemple(Guid id, [FromBody] UpdateTempleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Temple>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var temple = await _context.Temples.FindAsync(id);
            if (temple == null)
            {
                return NotFound(new ApiResponse<Temple>
                {
                    Success = false,
                    Message = "Temple not found"
                });
            }

            temple.Name = request.Name;
            temple.Description = request.Description;
            temple.Latitude = request.Latitude;
            temple.Longitude = request.Longitude;
            temple.Address = request.Address;
            temple.City = request.City;
            temple.State = request.State;
            temple.Country = request.Country;
            temple.PostalCode = request.PostalCode;
            temple.Type = request.Type;
            temple.MainDeity = request.MainDeity;
            temple.Significance = request.Significance;
            temple.History = request.History;
            temple.VisitingHours = request.VisitingHours;
            temple.ContactPhone = request.ContactPhone;
            temple.ContactEmail = request.ContactEmail;
            temple.Website = request.Website;
            temple.IsVerified = request.IsVerified;
            temple.UpdatedAt = DateTime.UtcNow;
            temple.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Temple>
            {
                Success = true,
                Message = "Temple updated successfully",
                Data = temple
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating temple with ID: {TempleId}", id);
            return StatusCode(500, new ApiResponse<Temple>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTemple(Guid id)
    {
        try
        {
            var temple = await _context.Temples.FindAsync(id);
            if (temple == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Temple not found"
                });
            }

            temple.IsActive = false;
            temple.UpdatedAt = DateTime.UtcNow;
            temple.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Temple deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting temple with ID: {TempleId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    private static decimal CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadius = 6371; // Earth's radius in kilometers
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return (decimal)(earthRadius * c);
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

// DTOs for Temple operations
public class CreateTempleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public TempleType Type { get; set; }
    public string? MainDeity { get; set; }
    public string? Significance { get; set; }
    public string? History { get; set; }
    public string? VisitingHours { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
    public bool IsVerified { get; set; } = false;
}

public class UpdateTempleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public TempleType Type { get; set; }
    public string? MainDeity { get; set; }
    public string? Significance { get; set; }
    public string? History { get; set; }
    public string? VisitingHours { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
    public bool IsVerified { get; set; }
}
