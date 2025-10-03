using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Services;

namespace AISanatanPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class AIDataAgentController : ControllerBase
{
    private readonly IAIDataAgentService _agentService;
    private readonly ILogger<AIDataAgentController> _logger;

    public AIDataAgentController(IAIDataAgentService agentService, ILogger<AIDataAgentController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Start the AI Data Agent
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<bool>>> StartAgent()
    {
        try
        {
            var result = await _agentService.StartAgentAsync();
            
            if (result)
            {
                _logger.LogInformation("AI Data Agent started by user: {User}", User.Identity?.Name);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "AI Data Agent started successfully",
                    Data = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to start AI Data Agent. It may already be running.",
                    Data = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting AI Data Agent");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while starting the AI Data Agent"
            });
        }
    }

    /// <summary>
    /// Stop the AI Data Agent
    /// </summary>
    [HttpPost("stop")]
    public async Task<ActionResult<ApiResponse<bool>>> StopAgent()
    {
        try
        {
            var result = await _agentService.StopAgentAsync();
            
            if (result)
            {
                _logger.LogInformation("AI Data Agent stopped by user: {User}", User.Identity?.Name);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "AI Data Agent stopped successfully",
                    Data = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to stop AI Data Agent. It may not be running.",
                    Data = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping AI Data Agent");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while stopping the AI Data Agent"
            });
        }
    }

    /// <summary>
    /// Pause the AI Data Agent
    /// </summary>
    [HttpPost("pause")]
    public async Task<ActionResult<ApiResponse<bool>>> PauseAgent()
    {
        try
        {
            var result = await _agentService.PauseAgentAsync();
            
            if (result)
            {
                _logger.LogInformation("AI Data Agent paused by user: {User}", User.Identity?.Name);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "AI Data Agent paused successfully",
                    Data = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to pause AI Data Agent. It may not be running.",
                    Data = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing AI Data Agent");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while pausing the AI Data Agent"
            });
        }
    }

    /// <summary>
    /// Resume the AI Data Agent
    /// </summary>
    [HttpPost("resume")]
    public async Task<ActionResult<ApiResponse<bool>>> ResumeAgent()
    {
        try
        {
            var result = await _agentService.ResumeAgentAsync();
            
            if (result)
            {
                _logger.LogInformation("AI Data Agent resumed by user: {User}", User.Identity?.Name);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "AI Data Agent resumed successfully",
                    Data = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to resume AI Data Agent. It may not be paused or running.",
                    Data = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming AI Data Agent");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while resuming the AI Data Agent"
            });
        }
    }

    /// <summary>
    /// Get the current status of the AI Data Agent
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<AgentStatus>>> GetStatus()
    {
        try
        {
            var status = await _agentService.GetStatusAsync();
            
            return Ok(new ApiResponse<AgentStatus>
            {
                Success = true,
                Message = "Agent status retrieved successfully",
                Data = status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI Data Agent status");
            return StatusCode(500, new ApiResponse<AgentStatus>
            {
                Success = false,
                Message = "An error occurred while retrieving the agent status"
            });
        }
    }

    /// <summary>
    /// Get the current progress of the AI Data Agent
    /// </summary>
    [HttpGet("progress")]
    public async Task<ActionResult<ApiResponse<CollectionProgress>>> GetProgress()
    {
        try
        {
            var progress = await _agentService.GetProgressAsync();
            
            return Ok(new ApiResponse<CollectionProgress>
            {
                Success = true,
                Message = "Agent progress retrieved successfully",
                Data = progress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI Data Agent progress");
            return StatusCode(500, new ApiResponse<CollectionProgress>
            {
                Success = false,
                Message = "An error occurred while retrieving the agent progress"
            });
        }
    }

    /// <summary>
    /// Check if the AI Data Agent is currently running
    /// </summary>
    [HttpGet("is-running")]
    public async Task<ActionResult<ApiResponse<bool>>> IsRunning()
    {
        try
        {
            var isRunning = await _agentService.IsRunningAsync();
            
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Agent running status retrieved successfully",
                Data = isRunning
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if AI Data Agent is running");
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while checking the agent status"
            });
        }
    }

    /// <summary>
    /// Manually trigger data collection for a specific type
    /// </summary>
    [HttpPost("collect/{dataType}")]
    public async Task<ActionResult<ApiResponse<bool>>> CollectData(string dataType)
    {
        try
        {
            var isRunning = await _agentService.IsRunningAsync();
            if (isRunning)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot start manual collection while agent is running. Please stop the agent first.",
                    Data = false
                });
            }

            bool result = false;
            switch (dataType.ToLower())
            {
                case "vedas":
                    await _agentService.CollectVedasDataAsync();
                    result = true;
                    break;
                case "puranas":
                    await _agentService.CollectPuranasDataAsync();
                    result = true;
                    break;
                case "kavyas":
                    await _agentService.CollectKavyasDataAsync();
                    result = true;
                    break;
                case "temples":
                    await _agentService.CollectTemplesDataAsync();
                    result = true;
                    break;
                case "mythological-places":
                    await _agentService.CollectMythologicalPlacesDataAsync();
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            if (result)
            {
                _logger.LogInformation("Manual data collection for {DataType} completed by user: {User}", 
                    dataType, User.Identity?.Name);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = $"Data collection for {dataType} completed successfully",
                    Data = true
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Invalid data type: {dataType}. Valid types are: vedas, puranas, kavyas, temples, mythological-places",
                    Data = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in manual data collection for {DataType}", dataType);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = $"An error occurred while collecting data for {dataType}"
            });
        }
    }
}
