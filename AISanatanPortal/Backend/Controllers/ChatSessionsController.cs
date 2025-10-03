using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatSessionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatSessionsController> _logger;

    public ChatSessionsController(ApplicationDbContext context, ILogger<ChatSessionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ChatSession>>>> GetChatSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] ChatSessionType? type = null,
        [FromQuery] ChatSessionStatus? status = null)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<PagedResult<ChatSession>>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var query = _context.ChatSessions
                .Where(cs => cs.UserId == userGuid)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(cs => cs.Title.Contains(search) || 
                                         cs.Context!.Contains(search) ||
                                         cs.Tags!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(cs => cs.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(cs => cs.Status == status.Value);
            }

            var totalCount = await query.CountAsync();
            var sessions = await query
                .OrderByDescending(cs => cs.LastMessageAt ?? cs.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<ChatSession>
            {
                Items = sessions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<ChatSession>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chat sessions");
            return StatusCode(500, new ApiResponse<PagedResult<ChatSession>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ChatSession>>> GetChatSession(Guid id)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var session = await _context.ChatSessions
                .Include(cs => cs.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.UserId == userGuid);

            if (session == null)
            {
                return NotFound(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Chat session not found"
                });
            }

            return Ok(new ApiResponse<ChatSession>
            {
                Success = true,
                Data = session
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chat session with ID: {SessionId}", id);
            return StatusCode(500, new ApiResponse<ChatSession>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ChatSession>>> CreateChatSession([FromBody] CreateChatSessionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var session = new ChatSession
            {
                UserId = userGuid,
                Title = request.Title,
                Type = request.Type,
                Status = request.Status,
                Context = request.Context,
                Tags = request.Tags,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChatSession), new { id = session.Id }, new ApiResponse<ChatSession>
            {
                Success = true,
                Message = "Chat session created successfully",
                Data = session
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chat session");
            return StatusCode(500, new ApiResponse<ChatSession>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ChatSession>>> UpdateChatSession(Guid id, [FromBody] UpdateChatSessionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.UserId == userGuid);

            if (session == null)
            {
                return NotFound(new ApiResponse<ChatSession>
                {
                    Success = false,
                    Message = "Chat session not found"
                });
            }

            session.Title = request.Title;
            session.Type = request.Type;
            session.Status = request.Status;
            session.Context = request.Context;
            session.Tags = request.Tags;
            session.UpdatedAt = DateTime.UtcNow;
            session.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<ChatSession>
            {
                Success = true,
                Message = "Chat session updated successfully",
                Data = session
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chat session with ID: {SessionId}", id);
            return StatusCode(500, new ApiResponse<ChatSession>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteChatSession(Guid id)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.UserId == userGuid);

            if (session == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Chat session not found"
                });
            }

            session.Status = ChatSessionStatus.Deleted;
            session.UpdatedAt = DateTime.UtcNow;
            session.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Chat session deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chat session with ID: {SessionId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/messages")]
    public async Task<ActionResult<ApiResponse<ChatMessage>>> AddMessage(Guid id, [FromBody] CreateMessageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ChatMessage>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<ChatMessage>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.UserId == userGuid);

            if (session == null)
            {
                return NotFound(new ApiResponse<ChatMessage>
                {
                    Success = false,
                    Message = "Chat session not found"
                });
            }

            var message = new ChatMessage
            {
                SessionId = id,
                Role = request.Role,
                Content = request.Content,
                Type = request.Type,
                Metadata = request.Metadata,
                ModelUsed = request.ModelUsed,
                TokensUsed = request.TokensUsed,
                ConfidenceScore = request.ConfidenceScore,
                Sources = request.Sources,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.ChatMessages.Add(message);

            // Update session
            session.MessageCount++;
            session.LastMessageAt = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<ChatMessage>
            {
                Success = true,
                Message = "Message added successfully",
                Data = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding message to chat session with ID: {SessionId}", id);
            return StatusCode(500, new ApiResponse<ChatMessage>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Chat operations
public class CreateChatSessionRequest
{
    public string Title { get; set; } = string.Empty;
    public ChatSessionType Type { get; set; } = ChatSessionType.General;
    public ChatSessionStatus Status { get; set; } = ChatSessionStatus.Active;
    public string? Context { get; set; }
    public string? Tags { get; set; }
}

public class UpdateChatSessionRequest
{
    public string Title { get; set; } = string.Empty;
    public ChatSessionType Type { get; set; }
    public ChatSessionStatus Status { get; set; }
    public string? Context { get; set; }
    public string? Tags { get; set; }
}

public class CreateMessageRequest
{
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public string? Metadata { get; set; }
    public string? ModelUsed { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public string? Sources { get; set; }
}
