using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Models;
using Xunit;

namespace AISanatanPortal.API.Tests;

public abstract class TestBase : IDisposable
{
    protected ApplicationDbContext Context { get; }
    
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        Context = new ApplicationDbContext(options);
        SeedTestData();
    }
    
    private void SeedTestData()
    {
        // Add test users
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        Context.Users.AddRange(testUser, adminUser);
        
        // Add test chat sessions
        var testSession = new ChatSession
        {
            Id = Guid.NewGuid(),
            UserId = testUser.Id,
            Title = "Test Chat Session",
            Type = ChatSessionType.General,
            Status = ChatSessionStatus.Active,
            MessageCount = 0,
            CreatedAt = DateTime.UtcNow
        };
        
        Context.ChatSessions.Add(testSession);
        
        // Add test assessments
        var testAssessment = new Assessment
        {
            Id = Guid.NewGuid(),
            Title = "Test Assessment",
            Description = "A test assessment for unit testing",
            Type = AssessmentType.Quiz,
            Category = AssessmentCategory.General,
            Difficulty = DifficultyLevel.Beginner,
            TimeLimit = 30,
            QuestionCount = 5,
            PassingScore = 70,
            IsActive = true,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow
        };
        
        Context.Assessments.Add(testAssessment);
        
        Context.SaveChanges();
    }
    
    protected async Task<T> AddEntityAsync<T>(T entity) where T : class
    {
        Context.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }
    
    protected async Task<List<T>> AddEntitiesAsync<T>(IEnumerable<T> entities) where T : class
    {
        Context.AddRange(entities);
        await Context.SaveChangesAsync();
        return entities.ToList();
    }
    
    protected async Task<T?> FindEntityAsync<T>(params object[] keyValues) where T : class
    {
        return await Context.FindAsync<T>(keyValues);
    }
    
    protected async Task<List<T>> GetAllEntitiesAsync<T>() where T : class
    {
        return await Context.Set<T>().ToListAsync();
    }
    
    protected async Task<int> CountEntitiesAsync<T>() where T : class
    {
        return await Context.Set<T>().CountAsync();
    }
    
    protected async Task ClearDatabaseAsync()
    {
        Context.RemoveRange(Context.Users);
        Context.RemoveRange(Context.ChatSessions);
        Context.RemoveRange(Context.Assessments);
        Context.RemoveRange(Context.Vedas);
        Context.RemoveRange(Context.Puranas);
        Context.RemoveRange(Context.Kavyas);
        Context.RemoveRange(Context.Temples);
        Context.RemoveRange(Context.MythologicalPlaces);
        
        await Context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}


