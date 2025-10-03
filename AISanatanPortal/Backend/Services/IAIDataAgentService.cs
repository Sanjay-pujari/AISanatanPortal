using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Services;

public interface IAIDataAgentService
{
    Task<bool> StartAgentAsync();
    Task<bool> StopAgentAsync();
    Task<bool> IsRunningAsync();
    Task<AgentStatus> GetStatusAsync();
    
    // Data collection methods
    Task CollectVedasDataAsync();
    Task CollectPuranasDataAsync();
    Task CollectKavyasDataAsync();
    Task CollectTemplesDataAsync();
    Task CollectMythologicalPlacesDataAsync();
    
    // Manual control methods
    Task<bool> PauseAgentAsync();
    Task<bool> ResumeAgentAsync();
    Task<CollectionProgress> GetProgressAsync();
}

public class AgentStatus
{
    public bool IsRunning { get; set; }
    public bool IsPaused { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? LastActivity { get; set; }
    public string CurrentTask { get; set; } = string.Empty;
    public int TotalRecordsProcessed { get; set; }
    public int TotalRecordsAdded { get; set; }
    public int TotalDuplicatesSkipped { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

public class CollectionProgress
{
    public string CurrentPhase { get; set; } = string.Empty;
    public int CurrentPhaseProgress { get; set; }
    public int TotalPhases { get; set; }
    public int CompletedPhases { get; set; }
    public List<string> CompletedTasks { get; set; } = new List<string>();
    public List<string> PendingTasks { get; set; } = new List<string>();
}
