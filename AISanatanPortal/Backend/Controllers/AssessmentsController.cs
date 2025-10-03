using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssessmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AssessmentsController> _logger;

    public AssessmentsController(ApplicationDbContext context, ILogger<AssessmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Assessment>>>> GetAssessments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] AssessmentType? type = null,
        [FromQuery] AssessmentCategory? category = null,
        [FromQuery] DifficultyLevel? difficulty = null,
        [FromQuery] bool? isPublic = null)
    {
        try
        {
            var query = _context.Assessments
                .Include(a => a.Questions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Title.Contains(search) || 
                                        a.Description!.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(a => a.Type == type.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(a => a.Category == category.Value);
            }

            if (difficulty.HasValue)
            {
                query = query.Where(a => a.Difficulty == difficulty.Value);
            }

            if (isPublic.HasValue)
            {
                query = query.Where(a => a.IsPublic == isPublic.Value);
            }

            var totalCount = await query.CountAsync();
            var assessments = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Assessment>
            {
                Items = assessments,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(new ApiResponse<PagedResult<Assessment>>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assessments");
            return StatusCode(500, new ApiResponse<PagedResult<Assessment>>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Assessment>>> GetAssessment(Guid id)
    {
        try
        {
            var assessment = await _context.Assessments
                .Include(a => a.Questions.OrderBy(q => q.DisplayOrder))
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
            {
                return NotFound(new ApiResponse<Assessment>
                {
                    Success = false,
                    Message = "Assessment not found"
                });
            }

            return Ok(new ApiResponse<Assessment>
            {
                Success = true,
                Data = assessment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assessment with ID: {AssessmentId}", id);
            return StatusCode(500, new ApiResponse<Assessment>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Assessment>>> CreateAssessment([FromBody] CreateAssessmentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Assessment>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var assessment = new Assessment
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                Category = request.Category,
                Difficulty = request.Difficulty,
                TimeLimit = request.TimeLimit,
                QuestionCount = request.Questions.Count,
                PassingScore = request.PassingScore,
                IsActive = request.IsActive,
                IsPublic = request.IsPublic,
                Instructions = request.Instructions,
                Tags = request.Tags,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            // Add questions
            foreach (var questionRequest in request.Questions)
            {
                var question = new Question
                {
                    AssessmentId = assessment.Id,
                    QuestionText = questionRequest.QuestionText,
                    Type = questionRequest.Type,
                    Options = questionRequest.Options,
                    CorrectAnswer = questionRequest.CorrectAnswer,
                    Points = questionRequest.Points,
                    DisplayOrder = questionRequest.DisplayOrder,
                    Explanation = questionRequest.Explanation,
                    Hint = questionRequest.Hint,
                    Difficulty = questionRequest.Difficulty,
                    Tags = questionRequest.Tags,
                    ImageUrl = questionRequest.ImageUrl,
                    IsActive = true,
                    CreatedBy = User.Identity?.Name,
                    UpdatedBy = User.Identity?.Name
                };

                _context.Questions.Add(question);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.Id }, new ApiResponse<Assessment>
            {
                Success = true,
                Message = "Assessment created successfully",
                Data = assessment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assessment");
            return StatusCode(500, new ApiResponse<Assessment>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<Assessment>>> UpdateAssessment(Guid id, [FromBody] UpdateAssessmentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Assessment>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound(new ApiResponse<Assessment>
                {
                    Success = false,
                    Message = "Assessment not found"
                });
            }

            assessment.Title = request.Title;
            assessment.Description = request.Description;
            assessment.Type = request.Type;
            assessment.Category = request.Category;
            assessment.Difficulty = request.Difficulty;
            assessment.TimeLimit = request.TimeLimit;
            assessment.PassingScore = request.PassingScore;
            assessment.IsActive = request.IsActive;
            assessment.IsPublic = request.IsPublic;
            assessment.Instructions = request.Instructions;
            assessment.Tags = request.Tags;
            assessment.UpdatedAt = DateTime.UtcNow;
            assessment.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Assessment>
            {
                Success = true,
                Message = "Assessment updated successfully",
                Data = assessment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assessment with ID: {AssessmentId}", id);
            return StatusCode(500, new ApiResponse<Assessment>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAssessment(Guid id)
    {
        try
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Assessment not found"
                });
            }

            assessment.IsActive = false;
            assessment.UpdatedAt = DateTime.UtcNow;
            assessment.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Assessment deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assessment with ID: {AssessmentId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("{id}/submit")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserAssessmentResult>>> SubmitAssessment(Guid id, [FromBody] SubmitAssessmentRequest request)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new ApiResponse<UserAssessmentResult>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var assessment = await _context.Assessments
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
            {
                return NotFound(new ApiResponse<UserAssessmentResult>
                {
                    Success = false,
                    Message = "Assessment not found"
                });
            }

            // Calculate score
            int score = 0;
            int maxScore = assessment.Questions.Sum(q => q.Points);

            foreach (var answer in request.Answers)
            {
                var question = assessment.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null && answer.Answer == question.CorrectAnswer)
                {
                    score += question.Points;
                }
            }

            decimal percentage = maxScore > 0 ? (decimal)score / maxScore * 100 : 0;
            bool isPassed = percentage >= assessment.PassingScore;

            var result = new UserAssessmentResult
            {
                UserId = userGuid,
                AssessmentId = id,
                Score = score,
                MaxScore = maxScore,
                Percentage = percentage,
                IsPassed = isPassed,
                TimeSpent = request.TimeSpent,
                StartedAt = request.StartedAt,
                CompletedAt = DateTime.UtcNow,
                Status = AssessmentResultStatus.Completed,
                Answers = System.Text.Json.JsonSerializer.Serialize(request.Answers),
                AttemptNumber = request.AttemptNumber,
                CreatedBy = User.Identity?.Name,
                UpdatedBy = User.Identity?.Name
            };

            _context.UserAssessmentResults.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserAssessmentResult>
            {
                Success = true,
                Message = "Assessment submitted successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting assessment with ID: {AssessmentId}", id);
            return StatusCode(500, new ApiResponse<UserAssessmentResult>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}

// DTOs for Assessment operations
public class CreateAssessmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssessmentType Type { get; set; }
    public AssessmentCategory Category { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int TimeLimit { get; set; } = 0;
    public int PassingScore { get; set; } = 70;
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public string? Instructions { get; set; }
    public string? Tags { get; set; }
    public List<CreateQuestionRequest> Questions { get; set; } = new List<CreateQuestionRequest>();
}

public class UpdateAssessmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssessmentType Type { get; set; }
    public AssessmentCategory Category { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public string? Instructions { get; set; }
    public string? Tags { get; set; }
}

public class CreateQuestionRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public string Options { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; } = 1;
    public int DisplayOrder { get; set; } = 0;
    public string? Explanation { get; set; }
    public string? Hint { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public string? Tags { get; set; }
    public string? ImageUrl { get; set; }
}

public class SubmitAssessmentRequest
{
    public DateTime StartedAt { get; set; }
    public int TimeSpent { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public List<AssessmentAnswer> Answers { get; set; } = new List<AssessmentAnswer>();
}

public class AssessmentAnswer
{
    public Guid QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}
