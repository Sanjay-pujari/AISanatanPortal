using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<User>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] UserRole? role = null)
    {
        try
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.FirstName.Contains(search) || 
                                        u.LastName.Contains(search) || 
                                        u.Email.Contains(search) || 
                                        u.Username.Contains(search));
            }

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<User>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new ApiResponse<PagedResult<User>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<User>>> GetUser(Guid id)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Preference)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse<User>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<User>
            {
                Success = true,
                Data = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<User>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<User>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    Message = "User with this email or username already exists"
                });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                IsEmailVerified = request.IsEmailVerified,
                ProfilePicture = request.ProfilePicture,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new ApiResponse<User>
            {
                Success = true,
                Message = "User created successfully",
                Data = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new ApiResponse<User>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<User>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<User>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.ProfilePicture = request.ProfilePicture;
            user.Role = request.Role;
            user.IsEmailVerified = request.IsEmailVerified;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<User>
            {
                Success = true,
                Message = "User updated successfully",
                Data = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<User>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "User deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> ActivateUser(Guid id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "User activated successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for User operations
public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsEmailVerified { get; set; } = false;
    public string? ProfilePicture { get; set; }
}

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? ProfilePicture { get; set; }
}
