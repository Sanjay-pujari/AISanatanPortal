using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using AISanatanPortal.API.Data;
using AISanatanPortal.API.Services;
using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Tests.Integration;

public class AIDataAgentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public AIDataAgentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
        
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testadmin",
            Email = "admin@test.com",
            FirstName = "Test",
            LastName = "Admin",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(testUser);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAgentStatus_ShouldReturnStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/aidataagent/status");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<AgentStatus>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAgentProgress_ShouldReturnProgress()
    {
        // Act
        var response = await _client.GetAsync("/api/aidataagent/progress");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<CollectionProgress>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CheckIfRunning_ShouldReturnBoolean()
    {
        // Act
        var response = await _client.GetAsync("/api/aidataagent/is-running");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.IsType<bool>(result.Data);
    }

    [Fact]
    public async Task StartAgent_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/start", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task StopAgent_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/stop", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PauseAgent_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/pause", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ResumeAgent_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/resume", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CollectData_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/collect/vedas", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CollectData_WithInvalidDataType_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PostAsync("/api/aidataagent/collect/invalid-type", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AgentService_ShouldBeRegistered()
    {
        // Act
        var service = _scope.ServiceProvider.GetService<IAIDataAgentService>();

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<IAIDataAgentService>(service);
    }

    [Fact]
    public async Task DatabaseConnection_ShouldWork()
    {
        // Act
        var userCount = await _context.Users.CountAsync();

        // Assert
        Assert.True(userCount >= 0);
    }

    [Fact]
    public async Task AgentStatus_ShouldHaveDefaultValues()
    {
        // Arrange
        var service = _scope.ServiceProvider.GetRequiredService<IAIDataAgentService>();

        // Act
        var status = await service.GetStatusAsync();

        // Assert
        Assert.NotNull(status);
        Assert.False(status.IsRunning);
        Assert.False(status.IsPaused);
        Assert.Empty(status.Errors);
        Assert.Equal(0, status.TotalRecordsProcessed);
        Assert.Equal(0, status.TotalRecordsAdded);
        Assert.Equal(0, status.TotalDuplicatesSkipped);
    }

    [Fact]
    public async Task CollectionProgress_ShouldHaveDefaultValues()
    {
        // Arrange
        var service = _scope.ServiceProvider.GetRequiredService<IAIDataAgentService>();

        // Act
        var progress = await service.GetProgressAsync();

        // Assert
        Assert.NotNull(progress);
        Assert.NotNull(progress.CompletedTasks);
        Assert.NotNull(progress.PendingTasks);
        Assert.Equal(0, progress.CurrentPhaseProgress);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _scope?.Dispose();
        _context?.Dispose();
    }
}


