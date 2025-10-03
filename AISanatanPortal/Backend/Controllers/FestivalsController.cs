using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FestivalsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FestivalsController> _logger;

    public FestivalsController(ApplicationDbContext context, ILogger<FestivalsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Festival>>>> GetFestivals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] FestivalType? type = null,
        [FromQuery] FestivalCategory? category = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool? isFeatured = null)
    {
        try
        {
            var query = _context.Festivals.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Name.Contains(search) || 
                                        f.SanskritName!.Contains(search) ||
                                        f.Description!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(f => f.Type == type.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(f => f.Category == category.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(f => f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(f => f.Date <= endDate.Value);
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(f => f.IsFeatured == isFeatured.Value);
            }

            var totalCount = await query.CountAsync();
            var festivals = await query
                .OrderBy(f => f.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Festival>
            {
                Items = festivals,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Festival>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving festivals");
            return StatusCode(500, new ApiResponse<PagedResult<Festival>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<ApiResponse<List<Festival>>>> GetUpcomingFestivals(
        [FromQuery] int count = 10)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var festivals = await _context.Festivals
                .Where(f => f.Date >= today && f.IsActive)
                .OrderBy(f => f.Date)
                .Take(count)
                .ToListAsync();

            return Ok(new ApiResponse<List<Festival>>
            {
                Success = true,
                Data = festivals
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming festivals");
            return StatusCode(500, new ApiResponse<List<Festival>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Festival>>> GetFestival(Guid id)
    {
        try
        {
            var festival = await _context.Festivals
                .Include(f => f.PanchangData)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (festival == null)
            {
                return NotFound(new ApiResponse<Festival>
                {
                    Success = false,
                    Message = "Festival not found"
                });
            }

            return Ok(new ApiResponse<Festival>
            {
                Success = true,
                Data = festival
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving festival with ID: {FestivalId}", id);
            return StatusCode(500, new ApiResponse<Festival>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Festival>>> CreateFestival([FromBody] CreateFestivalRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Festival>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var festival = new Festival
            {
                Name = request.Name,
                SanskritName = request.SanskritName,
                Description = request.Description,
                Content = request.Content,
                Type = request.Type,
                Category = request.Category,
                Date = request.Date,
                EndDate = request.EndDate,
                IsNationalHoliday = request.IsNationalHoliday,
                IsRegionalFestival = request.IsRegionalFestival,
                Regions = request.Regions,
                MainDeity = request.MainDeity,
                Rituals = request.Rituals,
                Significance = request.Significance,
                Traditions = request.Traditions,
                Foods = request.Foods,
                BannerImageUrl = request.BannerImageUrl,
                ImageUrls = request.ImageUrls,
                Tags = request.Tags,
                IsFeatured = request.IsFeatured,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Festivals.Add(festival);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFestival), new { id = festival.Id }, new ApiResponse<Festival>
            {
                Success = true,
                Message = "Festival created successfully",
                Data = festival
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating festival");
            return StatusCode(500, new ApiResponse<Festival>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Festival>>> UpdateFestival(Guid id, [FromBody] UpdateFestivalRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Festival>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var festival = await _context.Festivals.FindAsync(id);
            if (festival == null)
            {
                return NotFound(new ApiResponse<Festival>
                {
                    Success = false,
                    Message = "Festival not found"
                });
            }

            festival.Name = request.Name;
            festival.SanskritName = request.SanskritName;
            festival.Description = request.Description;
            festival.Content = request.Content;
            festival.Type = request.Type;
            festival.Category = request.Category;
            festival.Date = request.Date;
            festival.EndDate = request.EndDate;
            festival.IsNationalHoliday = request.IsNationalHoliday;
            festival.IsRegionalFestival = request.IsRegionalFestival;
            festival.Regions = request.Regions;
            festival.MainDeity = request.MainDeity;
            festival.Rituals = request.Rituals;
            festival.Significance = request.Significance;
            festival.Traditions = request.Traditions;
            festival.Foods = request.Foods;
            festival.BannerImageUrl = request.BannerImageUrl;
            festival.ImageUrls = request.ImageUrls;
            festival.Tags = request.Tags;
            festival.IsFeatured = request.IsFeatured;
            festival.UpdatedAt = DateTime.UtcNow;
            festival.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Festival>
            {
                Success = true,
                Message = "Festival updated successfully",
                Data = festival
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating festival with ID: {FestivalId}", id);
            return StatusCode(500, new ApiResponse<Festival>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFestival(Guid id)
    {
        try
        {
            var festival = await _context.Festivals.FindAsync(id);
            if (festival == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Festival not found"
                });
            }

            festival.IsActive = false;
            festival.UpdatedAt = DateTime.UtcNow;
            festival.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Festival deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting festival with ID: {FestivalId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Festival operations
public class CreateFestivalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public FestivalType Type { get; set; }
    public FestivalCategory Category { get; set; }
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsNationalHoliday { get; set; } = false;
    public bool IsRegionalFestival { get; set; } = false;
    public string? Regions { get; set; }
    public string? MainDeity { get; set; }
    public string? Rituals { get; set; }
    public string? Significance { get; set; }
    public string? Traditions { get; set; }
    public string? Foods { get; set; }
    public string? BannerImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public string? Tags { get; set; }
    public bool IsFeatured { get; set; } = false;
}

public class UpdateFestivalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? SanskritName { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public FestivalType Type { get; set; }
    public FestivalCategory Category { get; set; }
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsNationalHoliday { get; set; }
    public bool IsRegionalFestival { get; set; }
    public string? Regions { get; set; }
    public string? MainDeity { get; set; }
    public string? Rituals { get; set; }
    public string? Significance { get; set; }
    public string? Traditions { get; set; }
    public string? Foods { get; set; }
    public string? BannerImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public string? Tags { get; set; }
    public bool IsFeatured { get; set; }
}
