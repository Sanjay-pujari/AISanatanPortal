using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EventsController> _logger;

    public EventsController(ApplicationDbContext context, ILogger<EventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Event>>>> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] EventType? type = null,
        [FromQuery] EventCategory? category = null,
        [FromQuery] EventStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool? isFeatured = null)
    {
        try
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Title.Contains(search) || 
                                        e.SanskritTitle!.Contains(search) ||
                                        e.Description!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(e => e.Type == type.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(e => e.Category == category.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(e => e.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.StartDate <= endDate.Value);
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(e => e.IsFeatured == isFeatured.Value);
            }

            var totalCount = await query.CountAsync();
            var events = await query
                .OrderBy(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Event>
            {
                Items = events,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Event>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(500, new ApiResponse<PagedResult<Event>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Event>>> GetEvent(Guid id)
    {
        try
        {
            var eventEntity = await _context.Events
                .Include(e => e.Registrations)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                return NotFound(new ApiResponse<Event>
                {
                    Success = false,
                    Message = "Event not found"
                });
            }

            return Ok(new ApiResponse<Event>
            {
                Success = true,
                Data = eventEntity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event with ID: {EventId}", id);
            return StatusCode(500, new ApiResponse<Event>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Event>>> CreateEvent([FromBody] CreateEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Event>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var eventEntity = new Event
            {
                Title = request.Title,
                SanskritTitle = request.SanskritTitle,
                Description = request.Description,
                Content = request.Content,
                Type = request.Type,
                Category = request.Category,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsAllDay = request.IsAllDay,
                IsRecurring = request.IsRecurring,
                RecurrencePattern = request.RecurrencePattern,
                VenueName = request.VenueName,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsOnline = request.IsOnline,
                OnlineUrl = request.OnlineUrl,
                RequiresRegistration = request.RequiresRegistration,
                MaxAttendees = request.MaxAttendees,
                RegistrationFee = request.RegistrationFee,
                RegistrationDeadline = request.RegistrationDeadline,
                Status = request.Status,
                BannerImageUrl = request.BannerImageUrl,
                ImageUrls = request.ImageUrls,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                IsFeatured = request.IsFeatured,
                Tags = request.Tags,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.Id }, new ApiResponse<Event>
            {
                Success = true,
                Message = "Event created successfully",
                Data = eventEntity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, new ApiResponse<Event>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Event>>> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Event>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound(new ApiResponse<Event>
                {
                    Success = false,
                    Message = "Event not found"
                });
            }

            eventEntity.Title = request.Title;
            eventEntity.SanskritTitle = request.SanskritTitle;
            eventEntity.Description = request.Description;
            eventEntity.Content = request.Content;
            eventEntity.Type = request.Type;
            eventEntity.Category = request.Category;
            eventEntity.StartDate = request.StartDate;
            eventEntity.EndDate = request.EndDate;
            eventEntity.StartTime = request.StartTime;
            eventEntity.EndTime = request.EndTime;
            eventEntity.IsAllDay = request.IsAllDay;
            eventEntity.IsRecurring = request.IsRecurring;
            eventEntity.RecurrencePattern = request.RecurrencePattern;
            eventEntity.VenueName = request.VenueName;
            eventEntity.Address = request.Address;
            eventEntity.City = request.City;
            eventEntity.State = request.State;
            eventEntity.Country = request.Country;
            eventEntity.Latitude = request.Latitude;
            eventEntity.Longitude = request.Longitude;
            eventEntity.IsOnline = request.IsOnline;
            eventEntity.OnlineUrl = request.OnlineUrl;
            eventEntity.RequiresRegistration = request.RequiresRegistration;
            eventEntity.MaxAttendees = request.MaxAttendees;
            eventEntity.RegistrationFee = request.RegistrationFee;
            eventEntity.RegistrationDeadline = request.RegistrationDeadline;
            eventEntity.Status = request.Status;
            eventEntity.BannerImageUrl = request.BannerImageUrl;
            eventEntity.ImageUrls = request.ImageUrls;
            eventEntity.ContactPhone = request.ContactPhone;
            eventEntity.ContactEmail = request.ContactEmail;
            eventEntity.IsFeatured = request.IsFeatured;
            eventEntity.Tags = request.Tags;
            eventEntity.UpdatedAt = DateTime.UtcNow;
            eventEntity.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Event>
            {
                Success = true,
                Message = "Event updated successfully",
                Data = eventEntity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event with ID: {EventId}", id);
            return StatusCode(500, new ApiResponse<Event>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEvent(Guid id)
    {
        try
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Event not found"
                });
            }

            eventEntity.IsActive = false;
            eventEntity.UpdatedAt = DateTime.UtcNow;
            eventEntity.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Event deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event with ID: {EventId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/register")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EventRegistration>>> RegisterForEvent(Guid id, [FromBody] RegisterForEventRequest request)
    {
        try
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "Event not found"
                });
            }

            if (!eventEntity.RequiresRegistration)
            {
                return BadRequest(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "This event does not require registration"
                });
            }

            if (eventEntity.MaxAttendees.HasValue && eventEntity.CurrentAttendees >= eventEntity.MaxAttendees.Value)
            {
                return BadRequest(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "Event is full"
                });
            }

            if (eventEntity.RegistrationDeadline.HasValue && DateTime.UtcNow > eventEntity.RegistrationDeadline.Value)
            {
                return BadRequest(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "Registration deadline has passed"
                });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            // Check if user is already registered
            var existingRegistration = await _context.EventRegistrations
                .FirstOrDefaultAsync(er => er.EventId == id && er.UserId == userGuid);
            
            if (existingRegistration != null)
            {
                return BadRequest(new ApiResponse<EventRegistration>
                {
                    Success = false,
                    Message = "You are already registered for this event"
                });
            }

            var registration = new EventRegistration
            {
                EventId = id,
                UserId = userGuid,
                AttendeeFirstName = request.AttendeeFirstName,
                AttendeeLastName = request.AttendeeLastName,
                AttendeeEmail = request.AttendeeEmail,
                AttendeePhone = request.AttendeePhone,
                NumberOfAttendees = request.NumberOfAttendees,
                SpecialRequests = request.SpecialRequests,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.EventRegistrations.Add(registration);
            
            // Update event attendee count
            eventEntity.CurrentAttendees++;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<EventRegistration>
            {
                Success = true,
                Message = "Successfully registered for event",
                Data = registration
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering for event with ID: {EventId}", id);
            return StatusCode(500, new ApiResponse<EventRegistration>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Event operations
public class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? SanskritTitle { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public EventCategory Category { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public bool IsAllDay { get; set; } = false;
    public bool IsRecurring { get; set; } = false;
    public RecurrencePattern? RecurrencePattern { get; set; }
    public string? VenueName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsOnline { get; set; } = false;
    public string? OnlineUrl { get; set; }
    public bool RequiresRegistration { get; set; } = false;
    public int? MaxAttendees { get; set; }
    public decimal? RegistrationFee { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Upcoming;
    public string? BannerImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsFeatured { get; set; } = false;
    public string? Tags { get; set; }
}

public class UpdateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? SanskritTitle { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public EventCategory Category { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public bool IsRecurring { get; set; }
    public RecurrencePattern? RecurrencePattern { get; set; }
    public string? VenueName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineUrl { get; set; }
    public bool RequiresRegistration { get; set; }
    public int? MaxAttendees { get; set; }
    public decimal? RegistrationFee { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public EventStatus Status { get; set; }
    public string? BannerImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsFeatured { get; set; }
    public string? Tags { get; set; }
}

public class RegisterForEventRequest
{
    public string AttendeeFirstName { get; set; } = string.Empty;
    public string AttendeeLastName { get; set; } = string.Empty;
    public string AttendeeEmail { get; set; } = string.Empty;
    public string? AttendeePhone { get; set; }
    public int NumberOfAttendees { get; set; } = 1;
    public string? SpecialRequests { get; set; }
}
