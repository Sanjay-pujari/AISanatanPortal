using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Services;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<AuthResult>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse<AuthResult>
                {
                    Success = false,
                    Message = result.Message,
                    Data = result
                });
            }

            return Ok(new ApiResponse<AuthResult>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<AuthResult>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<AuthResult>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<AuthResult>
                {
                    Success = false,
                    Message = result.Message,
                    Data = result
                });
            }

            return Created("", new ApiResponse<AuthResult>
            {
                Success = true,
                Message = "Registration successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<AuthResult>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse<AuthResult>
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse<AuthResult>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new ApiResponse<AuthResult>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid user context"
                });
            }

            var result = await _authService.LogoutAsync(userId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Logout successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Email is required"
                });
            }

            var result = await _authService.ForgotPasswordAsync(email);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Password reset instructions have been sent to your email",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password request");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var result = await _authService.ResetPasswordAsync(request);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Password reset successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }

    [HttpGet("verify-email")]
    public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail([FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Verification token is required"
                });
            }

            var result = await _authService.VerifyEmailAsync(token);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Email verified successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An internal server error occurred"
            });
        }
    }
}