using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Preference)
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResult { Success = false, Message = "Invalid email or password." };
            }

            if (!user.IsActive)
            {
                return new AuthResult { Success = false, Message = "Account is deactivated." };
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ProfilePicture = user.ProfilePicture
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new AuthResult { Success = false, Message = "An error occurred during login." };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == request.Email || u.Username == request.Username);

            if (existingUser)
            {
                return new AuthResult { Success = false, Message = "User with this email or username already exists." };
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = UserRole.User
            };

            _context.Users.Add(user);

            // Create default preferences
            var preferences = new UserPreference
            {
                UserId = user.Id,
                Language = "en",
                Theme = "light",
                NotificationsEnabled = true,
                EmailUpdatesEnabled = true
            };

            _context.UserPreferences.Add(preferences);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResult
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Registration successful.",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return new AuthResult { Success = false, Message = "An error occurred during registration." };
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        // Implementation for refresh token logic
        // This would typically involve validating the refresh token and generating a new access token
        await Task.CompletedTask;
        return new AuthResult { Success = false, Message = "Refresh token functionality not implemented yet." };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        // Implementation for logout logic
        // This might involve blacklisting the token or updating user's last activity
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        // Implementation for email verification
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        // Implementation for forgot password logic
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Implementation for password reset logic
        await Task.CompletedTask;
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"]);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        // Using BCrypt or similar for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}