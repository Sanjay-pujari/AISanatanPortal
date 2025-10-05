using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AISanatanPortal.API.Controllers;
using AISanatanPortal.API.Services;
using AISanatanPortal.API.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AISanatanPortal.API.Tests;

public class AIDataAgentControllerTests
{
    private readonly Mock<IAIDataAgentService> _mockAgentService;
    private readonly Mock<ILogger<AIDataAgentController>> _mockLogger;
    private readonly AIDataAgentController _controller;

    public AIDataAgentControllerTests()
    {
        _mockAgentService = new Mock<IAIDataAgentService>();
        _mockLogger = new Mock<ILogger<AIDataAgentController>>();
        
        _controller = new AIDataAgentController(_mockAgentService.Object, _mockLogger.Object);
        
        // Setup controller context with admin user
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser"),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    [Fact]
    public async Task StartAgent_WhenSuccessful_ShouldReturnOkResult()
    {
        // Arrange
        _mockAgentService.Setup(x => x.StartAgentAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.StartAgent();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Equal("AI Data Agent started successfully", response.Message);
    }

    [Fact]
    public async Task StartAgent_WhenAlreadyRunning_ShouldReturnBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.StartAgentAsync()).ReturnsAsync(false);
        
        // Act
        var result = await _controller.StartAgent();
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.False(response.Data);
        Assert.Contains("already be running", response.Message);
    }

    [Fact]
    public async Task StartAgent_WhenException_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockAgentService.Setup(x => x.StartAgentAsync()).ThrowsAsync(new Exception("Test exception"));
        
        // Act
        var result = await _controller.StartAgent();
        
        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task StopAgent_WhenSuccessful_ShouldReturnOkResult()
    {
        // Arrange
        _mockAgentService.Setup(x => x.StopAgentAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.StopAgent();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Equal("AI Data Agent stopped successfully", response.Message);
    }

    [Fact]
    public async Task StopAgent_WhenNotRunning_ShouldReturnBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.StopAgentAsync()).ReturnsAsync(false);
        
        // Act
        var result = await _controller.StopAgent();
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.False(response.Data);
        Assert.Contains("may not be running", response.Message);
    }

    [Fact]
    public async Task PauseAgent_WhenSuccessful_ShouldReturnOkResult()
    {
        // Arrange
        _mockAgentService.Setup(x => x.PauseAgentAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.PauseAgent();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Equal("AI Data Agent paused successfully", response.Message);
    }

    [Fact]
    public async Task ResumeAgent_WhenSuccessful_ShouldReturnOkResult()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ResumeAgentAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.ResumeAgent();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Equal("AI Data Agent resumed successfully", response.Message);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnAgentStatus()
    {
        // Arrange
        var expectedStatus = new AgentStatus
        {
            IsRunning = true,
            IsPaused = false,
            StartTime = DateTime.UtcNow,
            CurrentTask = "Collecting Vedas Data",
            TotalRecordsProcessed = 10,
            TotalRecordsAdded = 8,
            TotalDuplicatesSkipped = 2,
            Errors = new List<string>()
        };
        
        _mockAgentService.Setup(x => x.GetStatusAsync()).ReturnsAsync(expectedStatus);
        
        // Act
        var result = await _controller.GetStatus();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<AgentStatus>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(expectedStatus, response.Data);
        Assert.Equal("Agent status retrieved successfully", response.Message);
    }

    [Fact]
    public async Task GetProgress_ShouldReturnCollectionProgress()
    {
        // Arrange
        var expectedProgress = new CollectionProgress
        {
            CurrentPhase = "Collecting Vedas Data",
            CurrentPhaseProgress = 75,
            TotalPhases = 5,
            CompletedPhases = 2,
            CompletedTasks = new List<string> { "Collecting Vedas Data", "Collecting Puranas Data" },
            PendingTasks = new List<string> { "Collecting Kavyas Data", "Collecting Temples Data", "Collecting Mythological Places Data" }
        };
        
        _mockAgentService.Setup(x => x.GetProgressAsync()).ReturnsAsync(expectedProgress);
        
        // Act
        var result = await _controller.GetProgress();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<CollectionProgress>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(expectedProgress, response.Data);
        Assert.Equal("Agent progress retrieved successfully", response.Message);
    }

    [Fact]
    public async Task IsRunning_ShouldReturnRunningStatus()
    {
        // Arrange
        _mockAgentService.Setup(x => x.IsRunningAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.IsRunning();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Equal("Agent running status retrieved successfully", response.Message);
    }

    [Theory]
    [InlineData("vedas")]
    [InlineData("puranas")]
    [InlineData("kavyas")]
    [InlineData("temples")]
    [InlineData("mythological-places")]
    public async Task CollectData_WithValidDataType_ShouldReturnOkResult(string dataType)
    {
        // Arrange
        _mockAgentService.Setup(x => x.IsRunningAsync()).ReturnsAsync(false);
        
        // Act
        var result = await _controller.CollectData(dataType);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Contains($"Data collection for {dataType} completed successfully", response.Message);
    }

    [Fact]
    public async Task CollectData_WhenAgentIsRunning_ShouldReturnBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.IsRunningAsync()).ReturnsAsync(true);
        
        // Act
        var result = await _controller.CollectData("vedas");
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.False(response.Data);
        Assert.Contains("Cannot start manual collection while agent is running", response.Message);
    }

    [Fact]
    public async Task CollectData_WithInvalidDataType_ShouldReturnBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.IsRunningAsync()).ReturnsAsync(false);
        
        // Act
        var result = await _controller.CollectData("invalid-type");
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.False(response.Data);
        Assert.Contains("Invalid data type", response.Message);
    }
}


