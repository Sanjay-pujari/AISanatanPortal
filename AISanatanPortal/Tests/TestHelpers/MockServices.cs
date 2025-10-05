using Moq;
using AISanatanPortal.API.Services;
using AISanatanPortal.API.Models;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Tests.TestHelpers;

public static class MockServices
{
    public static Mock<IWebScrapingService> CreateWebScrapingServiceMock()
    {
        var mock = new Mock<IWebScrapingService>();
        
        // Setup default successful responses
        mock.Setup(x => x.SearchWikipediaAsync(It.IsAny<string>()))
            .ReturnsAsync("Mock Wikipedia content for testing purposes. This contains information about the requested topic.");
            
        return mock;
    }

    public static Mock<IWebScrapingService> CreateWebScrapingServiceMockWithError()
    {
        var mock = new Mock<IWebScrapingService>();
        
        mock.Setup(x => x.SearchWikipediaAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Network error"));
            
        return mock;
    }

    public static Mock<IDataValidationService> CreateDataValidationServiceMock()
    {
        var mock = new Mock<IDataValidationService>();
        
        // Setup default successful responses
        mock.Setup(x => x.ExtractSanskritNameAsync(It.IsAny<string>()))
            .ReturnsAsync("मॉक संस्कृत नाम");
            
        mock.Setup(x => x.ExtractDescriptionAsync(It.IsAny<string>()))
            .ReturnsAsync("Mock description extracted from content for testing purposes.");
            
        mock.Setup(x => x.IsVedaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        mock.Setup(x => x.IsPuranaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        mock.Setup(x => x.IsKavyaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        mock.Setup(x => x.IsTempleDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(false);
            
        mock.Setup(x => x.IsMythologicalPlaceDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(false);
            
        return mock;
    }

    public static Mock<IDataValidationService> CreateDataValidationServiceMockWithDuplicates()
    {
        var mock = CreateDataValidationServiceMock();
        
        // Override to return duplicates
        mock.Setup(x => x.IsVedaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.IsPuranaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.IsKavyaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.IsTempleDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.IsMythologicalPlaceDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);
            
        return mock;
    }

    public static Mock<IAzureOpenAIService> CreateAzureOpenAIServiceMock()
    {
        var mock = new Mock<IAzureOpenAIService>();
        
        mock.Setup(x => x.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Mock AI response for testing purposes.");
            
        mock.Setup(x => x.GetSpiritualGuidanceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Mock spiritual guidance based on your question.");
            
        mock.Setup(x => x.ExplainConceptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Mock explanation of the concept at the requested difficulty level.");
            
        return mock;
    }

    public static Mock<IAzureOpenAIService> CreateAzureOpenAIServiceMockWithError()
    {
        var mock = new Mock<IAzureOpenAIService>();
        
        mock.Setup(x => x.GetChatCompletionAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Azure OpenAI service error"));
            
        mock.Setup(x => x.GetSpiritualGuidanceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Azure OpenAI service error"));
            
        mock.Setup(x => x.ExplainConceptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Azure OpenAI service error"));
            
        return mock;
    }

    public static Mock<IAuthService> CreateAuthServiceMock()
    {
        var mock = new Mock<IAuthService>();
        
        var successResult = new AuthResult
        {
            Success = true,
            Token = "mock-jwt-token",
            RefreshToken = "mock-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            }
        };
        
        mock.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(successResult);
            
        mock.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(successResult);
            
        mock.Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(successResult);
            
        mock.Setup(x => x.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
            
        return mock;
    }

    public static Mock<IAuthService> CreateAuthServiceMockWithError()
    {
        var mock = new Mock<IAuthService>();
        
        var errorResult = new AuthResult
        {
            Success = false,
            Message = "Authentication failed"
        };
        
        mock.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(errorResult);
            
        mock.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(errorResult);
            
        mock.Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(errorResult);
            
        mock.Setup(x => x.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
            
        return mock;
    }
}

public static class TestDataGenerator
{
    public static Veda CreateTestVeda(string name = "Test Veda")
    {
        return new Veda
        {
            Name = name,
            SanskritName = "परीक्षण वेद",
            Description = "A test Veda for unit testing purposes",
            ChapterCount = 10,
            VerseCount = 100,
            ImageUrl = "/images/test-veda.jpg",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Purana CreateTestPurana(string name = "Test Purana")
    {
        return new Purana
        {
            Name = name,
            SanskritName = "परीक्षण पुराण",
            Description = "A test Purana for unit testing purposes",
            Type = PuranaType.Mahapurana,
            ChapterCount = 20,
            StoryCount = 50,
            ImageUrl = "/images/test-purana.jpg",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Kavya CreateTestKavya(string name = "Test Kavya", string author = "Test Author")
    {
        return new Kavya
        {
            Name = name,
            SanskritName = "परीक्षण काव्य",
            Author = author,
            Description = "A test Kavya for unit testing purposes",
            Type = KavyaType.Epic,
            ChapterCount = 15,
            ImageUrl = "/images/test-kavya.jpg",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Temple CreateTestTemple(string name = "Test Temple")
    {
        return new Temple
        {
            Name = name,
            Description = "A test temple for unit testing purposes",
            Latitude = 28.6139m,
            Longitude = 77.2090m,
            City = "Delhi",
            State = "Delhi",
            Country = "India",
            Type = TempleType.Structural,
            MainDeity = "Shiva",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static MythologicalPlace CreateTestMythologicalPlace(string name = "Test Place")
    {
        return new MythologicalPlace
        {
            Name = name,
            Description = "A test mythological place for unit testing purposes",
            Latitude = 28.6139m,
            Longitude = 77.2090m,
            City = "Delhi",
            State = "Delhi",
            Country = "India",
            Type = MythologicalPlaceType.City,
            MythologicalSignificance = "Test mythological significance",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User CreateTestUser(string userName = "testuser", UserRole role = UserRole.User)
    {
        return new User
        {
            Username = userName,
            Email = $"{userName}@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ChatSession CreateTestChatSession(Guid userId, string title = "Test Chat")
    {
        return new ChatSession
        {
            UserId = userId,
            Title = title,
            Type = ChatSessionType.General,
            Status = ChatSessionStatus.Active,
            MessageCount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Assessment CreateTestAssessment(string title = "Test Assessment")
    {
        return new Assessment
        {
            Title = title,
            Description = "A test assessment for unit testing purposes",
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
    }
}


