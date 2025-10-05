using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using Xunit;

namespace AISanatanPortal.API.Tests.TestHelpers;

public static class TestUtilities
{
    public static ApplicationDbContext CreateInMemoryDbContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;
            
        return new ApplicationDbContext(options);
    }

    public static async Task SeedTestDataAsync(ApplicationDbContext context)
    {
        // Add test users
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "superadmin",
                Email = "superadmin@example.com",
                FirstName = "Super",
                LastName = "Admin",
                Role = UserRole.SuperAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Add test chat sessions
        var chatSessions = new List<ChatSession>
        {
            new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = users[0].Id,
                Title = "Test Chat Session 1",
                Type = ChatSessionType.General,
                Status = ChatSessionStatus.Active,
                MessageCount = 5,
                LastMessageAt = DateTime.UtcNow.AddMinutes(-10),
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = users[0].Id,
                Title = "Spiritual Guidance Session",
                Type = ChatSessionType.Spiritual_Guidance,
                Status = ChatSessionStatus.Active,
                MessageCount = 3,
                LastMessageAt = DateTime.UtcNow.AddMinutes(-5),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        context.ChatSessions.AddRange(chatSessions);
        await context.SaveChangesAsync();

        // Add test assessments
        var assessments = new List<Assessment>
        {
            new Assessment
            {
                Id = Guid.NewGuid(),
                Title = "Vedas Knowledge Test",
                Description = "Test your knowledge about the Vedas",
                Type = AssessmentType.Quiz,
                Category = AssessmentCategory.Vedas,
                Difficulty = DifficultyLevel.Beginner,
                TimeLimit = 30,
                QuestionCount = 10,
                PassingScore = 70,
                IsActive = true,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow
            },
            new Assessment
            {
                Id = Guid.NewGuid(),
                Title = "Puranas Advanced Test",
                Description = "Advanced test about Puranas",
                Type = AssessmentType.Test,
                Category = AssessmentCategory.Puranas,
                Difficulty = DifficultyLevel.Advanced,
                TimeLimit = 60,
                QuestionCount = 20,
                PassingScore = 80,
                IsActive = true,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Assessments.AddRange(assessments);
        await context.SaveChangesAsync();

        // Add test content data
        var vedas = new List<Veda>
        {
            new Veda
            {
                Name = "Rigveda",
                SanskritName = "ऋग्वेद",
                Description = "The oldest of the four Vedas",
                ChapterCount = 10,
                VerseCount = 1028,
                ImageUrl = "/images/vedas/rigveda.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new Veda
            {
                Name = "Samaveda",
                SanskritName = "सामवेद",
                Description = "The Veda of melodies and chants",
                ChapterCount = 2,
                VerseCount = 1875,
                ImageUrl = "/images/vedas/samaveda.jpg",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Vedas.AddRange(vedas);
        await context.SaveChangesAsync();

        var puranas = new List<Purana>
        {
            new Purana
            {
                Name = "Vishnu Purana",
                SanskritName = "विष्णु पुराण",
                Description = "One of the eighteen Mahapuranas",
                Type = PuranaType.Mahapurana,
                ChapterCount = 6,
                StoryCount = 100,
                ImageUrl = "/images/puranas/vishnu-purana.jpg",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Puranas.AddRange(puranas);
        await context.SaveChangesAsync();

        var temples = new List<Temple>
        {
            new Temple
            {
                Name = "Kashi Vishwanath Temple",
                Description = "One of the most famous Shiva temples",
                Latitude = 25.3110m,
                Longitude = 83.0106m,
                City = "Varanasi",
                State = "Uttar Pradesh",
                Country = "India",
                Type = TempleType.Ancient,
                MainDeity = "Shiva",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Temples.AddRange(temples);
        await context.SaveChangesAsync();
    }

    public static async Task ClearAllDataAsync(ApplicationDbContext context)
    {
        // Clear all tables in dependency order
        context.RemoveRange(context.UserAssessmentResults);
        context.RemoveRange(context.Questions);
        context.RemoveRange(context.Assessments);
        context.RemoveRange(context.ChatMessages);
        context.RemoveRange(context.ChatSessions);
        context.RemoveRange(context.MythologicalPlaces);
        context.RemoveRange(context.Temples);
        context.RemoveRange(context.Kavyas);
        context.RemoveRange(context.Puranas);
        context.RemoveRange(context.Vedas);
        context.RemoveRange(context.Users);

        await context.SaveChangesAsync();
    }

    public static string ToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public static T FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public static async Task<bool> WaitForConditionAsync(
        Func<Task<bool>> condition, 
        TimeSpan timeout, 
        TimeSpan interval)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        while (stopwatch.Elapsed < timeout)
        {
            if (await condition())
                return true;
                
            await Task.Delay(interval);
        }
        
        return false;
    }

    public static async Task<T> WaitForValueAsync<T>(
        Func<Task<T>> getValue,
        Predicate<T> condition,
        TimeSpan timeout,
        TimeSpan interval)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        while (stopwatch.Elapsed < timeout)
        {
            var value = await getValue();
            if (condition(value))
                return value;
                
            await Task.Delay(interval);
        }
        
        throw new TimeoutException($"Condition not met within {timeout.TotalSeconds} seconds");
    }

    public static string GenerateRandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomEmail()
    {
        return $"test_{GenerateRandomString(8)}@example.com";
    }

    public static Guid GenerateRandomGuid()
    {
        return Guid.NewGuid();
    }

    public static DateTime GenerateRandomDate(DateTime? start = null, DateTime? end = null)
    {
        var startDate = start ?? DateTime.UtcNow.AddYears(-1);
        var endDate = end ?? DateTime.UtcNow;
        var range = (endDate - startDate).TotalDays;
        var random = new Random();
        
        return startDate.AddDays(random.NextDouble() * range);
    }

    public static List<T> GenerateRandomList<T>(Func<T> generator, int count = 5)
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            list.Add(generator());
        }
        return list;
    }

    public static bool IsValidGuid(string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static void AssertValidEntity<T>(T entity) where T : BaseEntity
    {
        Assert.NotNull(entity);
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.True(entity.CreatedAt >= DateTime.UtcNow.AddMinutes(-1));
    }

    public static void AssertValidUser(User user)
    {
        AssertValidEntity(user);
        Assert.NotNull(user.Username);
        Assert.NotNull(user.Email);
        Assert.True(IsValidEmail(user.Email));
        Assert.NotNull(user.FirstName);
        Assert.NotNull(user.LastName);
        Assert.True(user.IsActive);
    }

    public static void AssertValidChatSession(ChatSession session)
    {
        AssertValidEntity(session);
        Assert.NotEqual(Guid.Empty, session.UserId);
        Assert.NotNull(session.Title);
        Assert.True(session.MessageCount >= 0);
    }

    public static void AssertValidAssessment(Assessment assessment)
    {
        AssertValidEntity(assessment);
        Assert.NotNull(assessment.Title);
        Assert.NotNull(assessment.Description);
        Assert.True(assessment.TimeLimit >= 0);
        Assert.True(assessment.QuestionCount >= 0);
        Assert.True(assessment.PassingScore >= 0 && assessment.PassingScore <= 100);
        Assert.True(assessment.IsActive);
    }
}


