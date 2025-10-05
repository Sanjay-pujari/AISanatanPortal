using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Services;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Tests;

public class AIDataAgentServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IWebScrapingService> _mockWebScrapingService;
    private readonly Mock<IDataValidationService> _mockValidationService;
    private readonly Mock<ILogger<AIDataAgentService>> _mockLogger;
    private readonly AIDataAgentService _service;

    public AIDataAgentServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        
        // Setup mocks
        _mockWebScrapingService = new Mock<IWebScrapingService>();
        _mockValidationService = new Mock<IDataValidationService>();
        _mockLogger = new Mock<ILogger<AIDataAgentService>>();
        
        // Setup mock behaviors
        _mockWebScrapingService
            .Setup(x => x.SearchWikipediaAsync(It.IsAny<string>()))
            .ReturnsAsync("Mock Wikipedia content for testing");
            
        _mockValidationService
            .Setup(x => x.ExtractSanskritNameAsync(It.IsAny<string>()))
            .ReturnsAsync("मॉक संस्कृत नाम");
            
        _mockValidationService
            .Setup(x => x.ExtractDescriptionAsync(It.IsAny<string>()))
            .ReturnsAsync("Mock description for testing purposes");
            
        _mockValidationService
            .Setup(x => x.IsVedaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        _mockValidationService
            .Setup(x => x.IsPuranaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        _mockValidationService
            .Setup(x => x.IsKavyaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
            
        _mockValidationService
            .Setup(x => x.IsTempleDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(false);
            
        _mockValidationService
            .Setup(x => x.IsMythologicalPlaceDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(false);
        
        _service = new AIDataAgentService(
            _context,
            _mockWebScrapingService.Object,
            _mockValidationService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task StartAgentAsync_WhenNotRunning_ShouldReturnTrue()
    {
        // Act
        var result = await _service.StartAgentAsync();
        
        // Assert
        Assert.True(result);
        Assert.True(await _service.IsRunningAsync());
    }

    [Fact]
    public async Task StartAgentAsync_WhenAlreadyRunning_ShouldReturnFalse()
    {
        // Arrange
        await _service.StartAgentAsync();
        
        // Act
        var result = await _service.StartAgentAsync();
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task StopAgentAsync_WhenRunning_ShouldReturnTrue()
    {
        // Arrange
        await _service.StartAgentAsync();
        
        // Act
        var result = await _service.StopAgentAsync();
        
        // Assert
        Assert.True(result);
        Assert.False(await _service.IsRunningAsync());
    }

    [Fact]
    public async Task StopAgentAsync_WhenNotRunning_ShouldReturnFalse()
    {
        // Act
        var result = await _service.StopAgentAsync();
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PauseAgentAsync_WhenRunning_ShouldReturnTrue()
    {
        // Arrange
        await _service.StartAgentAsync();
        
        // Act
        var result = await _service.PauseAgentAsync();
        
        // Assert
        Assert.True(result);
        
        var status = await _service.GetStatusAsync();
        Assert.True(status.IsPaused);
    }

    [Fact]
    public async Task ResumeAgentAsync_WhenPaused_ShouldReturnTrue()
    {
        // Arrange
        await _service.StartAgentAsync();
        await _service.PauseAgentAsync();
        
        // Act
        var result = await _service.ResumeAgentAsync();
        
        // Assert
        Assert.True(result);
        
        var status = await _service.GetStatusAsync();
        Assert.False(status.IsPaused);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnCurrentStatus()
    {
        // Arrange
        await _service.StartAgentAsync();
        
        // Act
        var status = await _service.GetStatusAsync();
        
        // Assert
        Assert.NotNull(status);
        Assert.True(status.IsRunning);
        Assert.False(status.IsPaused);
        Assert.NotNull(status.StartTime);
    }

    [Fact]
    public async Task GetProgressAsync_ShouldReturnProgressInformation()
    {
        // Arrange
        await _service.StartAgentAsync();
        
        // Act
        var progress = await _service.GetProgressAsync();
        
        // Assert
        Assert.NotNull(progress);
        Assert.True(progress.TotalPhases > 0);
        Assert.NotNull(progress.PendingTasks);
        Assert.NotNull(progress.CompletedTasks);
    }

    [Fact]
    public async Task CollectVedasDataAsync_ShouldProcessVedasData()
    {
        // Act
        await _service.CollectVedasDataAsync();
        
        // Assert
        _mockWebScrapingService.Verify(
            x => x.SearchWikipediaAsync(It.IsAny<string>()), 
            Times.AtLeast(4) // Should search for all 4 Vedas
        );
        
        _mockValidationService.Verify(
            x => x.IsVedaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()), 
            Times.AtLeast(4)
        );
    }

    [Fact]
    public async Task CollectPuranasDataAsync_ShouldProcessPuranasData()
    {
        // Act
        await _service.CollectPuranasDataAsync();
        
        // Assert
        _mockWebScrapingService.Verify(
            x => x.SearchWikipediaAsync(It.IsAny<string>()), 
            Times.AtLeast(6) // Should search for 6 Puranas
        );
        
        _mockValidationService.Verify(
            x => x.IsPuranaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()), 
            Times.AtLeast(6)
        );
    }

    [Fact]
    public async Task CollectTemplesDataAsync_ShouldProcessTemplesData()
    {
        // Act
        await _service.CollectTemplesDataAsync();
        
        // Assert
        _mockWebScrapingService.Verify(
            x => x.SearchWikipediaAsync(It.IsAny<string>()), 
            Times.AtLeast(5) // Should search for 5 temples
        );
        
        _mockValidationService.Verify(
            x => x.IsTempleDuplicateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()), 
            Times.AtLeast(5)
        );
    }

    [Fact]
    public async Task CollectData_WithDuplicateDetection_ShouldSkipDuplicates()
    {
        // Arrange
        _mockValidationService
            .Setup(x => x.IsVedaDuplicateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true); // Simulate duplicate
        
        // Act
        await _service.CollectVedasDataAsync();
        
        // Assert
        var status = await _service.GetStatusAsync();
        Assert.True(status.TotalDuplicatesSkipped > 0);
    }

    [Fact]
    public async Task CollectData_WithWebScrapingError_ShouldHandleGracefully()
    {
        // Arrange
        _mockWebScrapingService
            .Setup(x => x.SearchWikipediaAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Network error"));
        
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.CollectVedasDataAsync());
        
        var status = await _service.GetStatusAsync();
        Assert.True(status.Errors.Count > 0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


