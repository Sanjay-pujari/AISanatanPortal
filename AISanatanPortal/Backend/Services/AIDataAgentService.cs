using AISanatanPortal.API.Models;
using AISanatanPortal.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AISanatanPortal.API.Services;

public class AIDataAgentService : IAIDataAgentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebScrapingService _webScrapingService;
    private readonly IDataValidationService _validationService;
    private readonly ILogger<AIDataAgentService> _logger;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly object _statusLock = new();
    private AgentStatus _status = new();
    private CollectionProgress _progress = new();

    public AIDataAgentService(
        ApplicationDbContext context,
        IWebScrapingService webScrapingService,
        IDataValidationService validationService,
        ILogger<AIDataAgentService> logger)
    {
        _context = context;
        _webScrapingService = webScrapingService;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<bool> StartAgentAsync()
    {
        lock (_statusLock)
        {
            if (_status.IsRunning)
                return false;
                
            _status.IsRunning = true;
            _status.IsPaused = false;
            _status.StartTime = DateTime.UtcNow;
            _status.CurrentTask = "Initializing AI Data Agent";
            _status.Errors.Clear();
        }

        _logger.LogInformation("AI Data Agent started at {StartTime}", DateTime.UtcNow);

        try
        {
            // Start the data collection process in background
            _ = Task.Run(async () => await RunDataCollectionAsync(_cancellationTokenSource.Token));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting AI Data Agent");
            lock (_statusLock)
            {
                _status.IsRunning = false;
                _status.Errors.Add($"Failed to start agent: {ex.Message}");
            }
            return false;
        }
    }

    public async Task<bool> StopAgentAsync()
    {
        lock (_statusLock)
        {
            if (!_status.IsRunning)
                return false;
        }

        try
        {
            _cancellationTokenSource.Cancel();
            
            lock (_statusLock)
            {
                _status.IsRunning = false;
                _status.IsPaused = false;
                _status.CurrentTask = "Stopped";
            }

            _logger.LogInformation("AI Data Agent stopped at {StopTime}", DateTime.UtcNow);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping AI Data Agent");
            return false;
        }
    }

    public async Task<bool> IsRunningAsync()
    {
        lock (_statusLock)
        {
            return _status.IsRunning;
        }
    }

    public async Task<AgentStatus> GetStatusAsync()
    {
        lock (_statusLock)
        {
            return new AgentStatus
            {
                IsRunning = _status.IsRunning,
                IsPaused = _status.IsPaused,
                StartTime = _status.StartTime,
                LastActivity = _status.LastActivity,
                CurrentTask = _status.CurrentTask,
                TotalRecordsProcessed = _status.TotalRecordsProcessed,
                TotalRecordsAdded = _status.TotalRecordsAdded,
                TotalDuplicatesSkipped = _status.TotalDuplicatesSkipped,
                Errors = new List<string>(_status.Errors)
            };
        }
    }

    public async Task<bool> PauseAgentAsync()
    {
        lock (_statusLock)
        {
            if (!_status.IsRunning)
                return false;
                
            _status.IsPaused = true;
            _status.CurrentTask = "Paused";
        }

        _logger.LogInformation("AI Data Agent paused");
        return true;
    }

    public async Task<bool> ResumeAgentAsync()
    {
        lock (_statusLock)
        {
            if (!_status.IsRunning || !_status.IsPaused)
                return false;
                
            _status.IsPaused = false;
            _status.CurrentTask = "Resuming";
        }

        _logger.LogInformation("AI Data Agent resumed");
        return true;
    }

    public async Task<CollectionProgress> GetProgressAsync()
    {
        return new CollectionProgress
        {
            CurrentPhase = _progress.CurrentPhase,
            CurrentPhaseProgress = _progress.CurrentPhaseProgress,
            TotalPhases = _progress.TotalPhases,
            CompletedPhases = _progress.CompletedPhases,
            CompletedTasks = new List<string>(_progress.CompletedTasks),
            PendingTasks = new List<string>(_progress.PendingTasks)
        };
    }

    private async Task RunDataCollectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var phases = new[]
            {
                "Collecting Vedas Data",
                "Collecting Puranas Data", 
                "Collecting Kavyas Data",
                "Collecting Temples Data",
                "Collecting Mythological Places Data"
            };

            _progress.TotalPhases = phases.Length;
            _progress.PendingTasks = phases.ToList();

            for (int i = 0; i < phases.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Check if paused
                while (await IsPausedAsync())
                {
                    await Task.Delay(1000, cancellationToken);
                }

                _progress.CurrentPhase = phases[i];
                _progress.CurrentPhaseProgress = 0;

                try
                {
                    switch (i)
                    {
                        case 0:
                            await CollectVedasDataAsync();
                            break;
                        case 1:
                            await CollectPuranasDataAsync();
                            break;
                        case 2:
                            await CollectKavyasDataAsync();
                            break;
                        case 3:
                            await CollectTemplesDataAsync();
                            break;
                        case 4:
                            await CollectMythologicalPlacesDataAsync();
                            break;
                    }

                    _progress.CompletedPhases++;
                    _progress.CompletedTasks.Add(phases[i]);
                    _progress.PendingTasks.Remove(phases[i]);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in phase: {Phase}", phases[i]);
                    lock (_statusLock)
                    {
                        _status.Errors.Add($"Error in {phases[i]}: {ex.Message}");
                    }
                }
            }

            lock (_statusLock)
            {
                _status.IsRunning = false;
                _status.CurrentTask = "Completed";
                _status.LastActivity = DateTime.UtcNow;
            }

            _logger.LogInformation("AI Data Agent completed all phases");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in AI Data Agent");
            lock (_statusLock)
            {
                _status.IsRunning = false;
                _status.Errors.Add($"Fatal error: {ex.Message}");
            }
        }
    }

    private async Task<bool> IsPausedAsync()
    {
        lock (_statusLock)
        {
            return _status.IsPaused;
        }
    }

    public async Task CollectVedasDataAsync()
    {
        _logger.LogInformation("Starting Vedas data collection");
        
        var vedaQueries = new[]
        {
            "Rigveda",
            "Samaveda", 
            "Yajurveda",
            "Atharvaveda"
        };

        foreach (var query in vedaQueries)
        {
            try
            {
                var content = await _webScrapingService.SearchWikipediaAsync(query);
                if (!string.IsNullOrEmpty(content))
                {
                    var veda = await ProcessVedaDataAsync(query, content);
                    if (veda != null)
                    {
                        var isDuplicate = await _validationService.IsVedaDuplicateAsync(veda.Name, veda.SanskritName ?? "");
                        if (!isDuplicate)
                        {
                            _context.Vedas.Add(veda);
                            await _context.SaveChangesAsync();
                            
                            lock (_statusLock)
                            {
                                _status.TotalRecordsAdded++;
                            }
                            
                            _logger.LogInformation("Added Veda: {Name}", veda.Name);
                        }
                        else
                        {
                            lock (_statusLock)
                            {
                                _status.TotalDuplicatesSkipped++;
                            }
                        }
                    }
                }
                
                lock (_statusLock)
                {
                    _status.TotalRecordsProcessed++;
                    _status.LastActivity = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Veda: {Query}", query);
            }
        }
    }

    public async Task CollectPuranasDataAsync()
    {
        _logger.LogInformation("Starting Puranas data collection");
        
        var puranaQueries = new[]
        {
            "Vishnu Purana",
            "Shiva Purana",
            "Bhagavata Purana",
            "Brahma Purana",
            "Matsya Purana",
            "Kurma Purana"
        };

        foreach (var query in puranaQueries)
        {
            try
            {
                var content = await _webScrapingService.SearchWikipediaAsync(query);
                if (!string.IsNullOrEmpty(content))
                {
                    var purana = await ProcessPuranaDataAsync(query, content);
                    if (purana != null)
                    {
                        var isDuplicate = await _validationService.IsPuranaDuplicateAsync(purana.Name, purana.SanskritName ?? "");
                        if (!isDuplicate)
                        {
                            _context.Puranas.Add(purana);
                            await _context.SaveChangesAsync();
                            
                            lock (_statusLock)
                            {
                                _status.TotalRecordsAdded++;
                            }
                            
                            _logger.LogInformation("Added Purana: {Name}", purana.Name);
                        }
                        else
                        {
                            lock (_statusLock)
                            {
                                _status.TotalDuplicatesSkipped++;
                            }
                        }
                    }
                }
                
                lock (_statusLock)
                {
                    _status.TotalRecordsProcessed++;
                    _status.LastActivity = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Purana: {Query}", query);
            }
        }
    }

    public async Task CollectKavyasDataAsync()
    {
        _logger.LogInformation("Starting Kavyas data collection");
        
        var kavyaQueries = new[]
        {
            "Ramayana Valmiki",
            "Mahabharata Vyasa",
            "Abhijnanasakuntalam Kalidasa",
            "Meghaduta Kalidasa",
            "Kumarasambhava Kalidasa",
            "Raghuvamsha Kalidasa"
        };

        foreach (var query in kavyaQueries)
        {
            try
            {
                var content = await _webScrapingService.SearchWikipediaAsync(query);
                if (!string.IsNullOrEmpty(content))
                {
                    var kavya = await ProcessKavyaDataAsync(query, content);
                    if (kavya != null)
                    {
                        var isDuplicate = await _validationService.IsKavyaDuplicateAsync(kavya.Name, kavya.Author);
                        if (!isDuplicate)
                        {
                            _context.Kavyas.Add(kavya);
                            await _context.SaveChangesAsync();
                            
                            lock (_statusLock)
                            {
                                _status.TotalRecordsAdded++;
                            }
                            
                            _logger.LogInformation("Added Kavya: {Name}", kavya.Name);
                        }
                        else
                        {
                            lock (_statusLock)
                            {
                                _status.TotalDuplicatesSkipped++;
                            }
                        }
                    }
                }
                
                lock (_statusLock)
                {
                    _status.TotalRecordsProcessed++;
                    _status.LastActivity = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kavya: {Query}", query);
            }
        }
    }

    public async Task CollectTemplesDataAsync()
    {
        _logger.LogInformation("Starting Temples data collection");
        
        var templeQueries = new[]
        {
            "Kashi Vishwanath Temple Varanasi",
            "Tirumala Venkateswara Temple",
            "Golden Temple Amritsar",
            "Meenakshi Amman Temple Madurai",
            "Jagannath Temple Puri"
        };

        foreach (var query in templeQueries)
        {
            try
            {
                var content = await _webScrapingService.SearchWikipediaAsync(query);
                if (!string.IsNullOrEmpty(content))
                {
                    var temple = await ProcessTempleDataAsync(query, content);
                    if (temple != null)
                    {
                        var isDuplicate = await _validationService.IsTempleDuplicateAsync(temple.Name, temple.Latitude, temple.Longitude);
                        if (!isDuplicate)
                        {
                            _context.Temples.Add(temple);
                            await _context.SaveChangesAsync();
                            
                            lock (_statusLock)
                            {
                                _status.TotalRecordsAdded++;
                            }
                            
                            _logger.LogInformation("Added Temple: {Name}", temple.Name);
                        }
                        else
                        {
                            lock (_statusLock)
                            {
                                _status.TotalDuplicatesSkipped++;
                            }
                        }
                    }
                }
                
                lock (_statusLock)
                {
                    _status.TotalRecordsProcessed++;
                    _status.LastActivity = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Temple: {Query}", query);
            }
        }
    }

    public async Task CollectMythologicalPlacesDataAsync()
    {
        _logger.LogInformation("Starting Mythological Places data collection");
        
        var placeQueries = new[]
        {
            "Ayodhya Rama birthplace",
            "Kurukshetra Mahabharata battlefield",
            "Dwarka Krishna kingdom",
            "Mount Kailash Shiva abode",
            "Vrindavan Krishna playground"
        };

        foreach (var query in placeQueries)
        {
            try
            {
                var content = await _webScrapingService.SearchWikipediaAsync(query);
                if (!string.IsNullOrEmpty(content))
                {
                    var place = await ProcessMythologicalPlaceDataAsync(query, content);
                    if (place != null)
                    {
                        var isDuplicate = await _validationService.IsMythologicalPlaceDuplicateAsync(place.Name, place.Latitude, place.Longitude);
                        if (!isDuplicate)
                        {
                            _context.MythologicalPlaces.Add(place);
                            await _context.SaveChangesAsync();
                            
                            lock (_statusLock)
                            {
                                _status.TotalRecordsAdded++;
                            }
                            
                            _logger.LogInformation("Added Mythological Place: {Name}", place.Name);
                        }
                        else
                        {
                            lock (_statusLock)
                            {
                                _status.TotalDuplicatesSkipped++;
                            }
                        }
                    }
                }
                
                lock (_statusLock)
                {
                    _status.TotalRecordsProcessed++;
                    _status.LastActivity = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Mythological Place: {Query}", query);
            }
        }
    }

    private async Task<Veda?> ProcessVedaDataAsync(string query, string content)
    {
        try
        {
            var name = query;
            var sanskritName = await _validationService.ExtractSanskritNameAsync(content);
            var description = await _validationService.ExtractDescriptionAsync(content);
            
            // Extract chapter and verse counts from content
            var chapterCount = ExtractNumberFromContent(content, "chapter", "mandala");
            var verseCount = ExtractNumberFromContent(content, "verse", "mantra");

            return new Veda
            {
                Name = name,
                SanskritName = sanskritName,
                Description = description,
                ChapterCount = chapterCount,
                VerseCount = verseCount,
                ImageUrl = $"/images/vedas/{name.ToLower().Replace(" ", "-")}.jpg"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Veda data for query: {Query}", query);
            return null;
        }
    }

    private async Task<Purana?> ProcessPuranaDataAsync(string query, string content)
    {
        try
        {
            var name = query;
            var sanskritName = await _validationService.ExtractSanskritNameAsync(content);
            var description = await _validationService.ExtractDescriptionAsync(content);
            
            var chapterCount = ExtractNumberFromContent(content, "chapter");
            var storyCount = ExtractNumberFromContent(content, "story", "legend");

            return new Purana
            {
                Name = name,
                SanskritName = sanskritName,
                Description = description,
                Type = PuranaType.Mahapurana,
                ChapterCount = chapterCount,
                StoryCount = storyCount,
                ImageUrl = $"/images/puranas/{name.ToLower().Replace(" ", "-")}.jpg"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Purana data for query: {Query}", query);
            return null;
        }
    }

    private async Task<Kavya?> ProcessKavyaDataAsync(string query, string content)
    {
        try
        {
            var parts = query.Split(' ');
            var name = parts[0];
            var author = parts.Length > 1 ? parts[1] : "Unknown";
            var sanskritName = await _validationService.ExtractSanskritNameAsync(content);
            var description = await _validationService.ExtractDescriptionAsync(content);
            
            var chapterCount = ExtractNumberFromContent(content, "chapter", "kanda", "parva");
            
            var type = DetermineKavyaType(name, content);

            return new Kavya
            {
                Name = name,
                SanskritName = sanskritName,
                Author = author,
                Description = description,
                Type = type,
                ChapterCount = chapterCount,
                ImageUrl = $"/images/kavyas/{name.ToLower().Replace(" ", "-")}.jpg"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Kavya data for query: {Query}", query);
            return null;
        }
    }

    private async Task<Temple?> ProcessTempleDataAsync(string query, string content)
    {
        try
        {
            var name = query.Split(' ')[0] + " " + query.Split(' ')[1]; // Get first two words
            var description = await _validationService.ExtractDescriptionAsync(content);
            
            // Extract location information
            var (latitude, longitude, city, state, country) = ExtractLocationFromContent(content);
            
            // Extract temple type
            var type = DetermineTempleType(content);
            
            // Extract main deity
            var mainDeity = ExtractMainDeityFromContent(content);

            return new Temple
            {
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                City = city,
                State = state,
                Country = country,
                Type = type,
                MainDeity = mainDeity,
                IsVerified = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Temple data for query: {Query}", query);
            return null;
        }
    }

    private async Task<MythologicalPlace?> ProcessMythologicalPlaceDataAsync(string query, string content)
    {
        try
        {
            var name = query.Split(' ')[0];
            var description = await _validationService.ExtractDescriptionAsync(content);
            
            // Extract location information
            var (latitude, longitude, city, state, country) = ExtractLocationFromContent(content);
            
            // Extract mythological significance
            var significance = ExtractMythologicalSignificance(content);
            
            // Extract place type
            var type = DetermineMythologicalPlaceType(content);

            return new MythologicalPlace
            {
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                City = city,
                State = state,
                Country = country,
                Type = type,
                MythologicalSignificance = significance,
                IsVerified = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Mythological Place data for query: {Query}", query);
            return null;
        }
    }

    private int ExtractNumberFromContent(string content, params string[] keywords)
    {
        foreach (var keyword in keywords)
        {
            var pattern = $@"{keyword}[^\d]*(\d+)";
            var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var number))
            {
                return number;
            }
        }
        return 0;
    }

    private KavyaType DetermineKavyaType(string name, string content)
    {
        if (name.Contains("Ramayana") || name.Contains("Mahabharata"))
            return KavyaType.Epic;
        if (name.Contains("Abhijnanasakuntalam"))
            return KavyaType.Drama;
        if (name.Contains("Meghaduta"))
            return KavyaType.Poetry;
        return KavyaType.Other;
    }

    private TempleType DetermineTempleType(string content)
    {
        if (content.Contains("ancient") || content.Contains("old"))
            return TempleType.Ancient;
        if (content.Contains("modern"))
            return TempleType.Modern;
        if (content.Contains("cave"))
            return TempleType.Cave;
        return TempleType.Structural;
    }

    private MythologicalPlaceType DetermineMythologicalPlaceType(string content)
    {
        if (content.Contains("birthplace") || content.Contains("birth"))
            return MythologicalPlaceType.Birthplace;
        if (content.Contains("battlefield") || content.Contains("war"))
            return MythologicalPlaceType.Battlefield;
        if (content.Contains("kingdom") || content.Contains("capital"))
            return MythologicalPlaceType.Kingdom;
        if (content.Contains("mountain") || content.Contains("hill"))
            return MythologicalPlaceType.Mountain;
        if (content.Contains("forest") || content.Contains("grove"))
            return MythologicalPlaceType.Forest;
        return MythologicalPlaceType.City;
    }

    private string ExtractMainDeityFromContent(string content)
    {
        var deities = new[] { "Shiva", "Vishnu", "Krishna", "Rama", "Ganesha", "Durga", "Lakshmi" };
        foreach (var deity in deities)
        {
            if (content.Contains(deity, StringComparison.OrdinalIgnoreCase))
                return deity;
        }
        return "Unknown";
    }

    private string ExtractMythologicalSignificance(string content)
    {
        // Extract sentences that contain mythological significance
        var sentences = content.Split('.');
        foreach (var sentence in sentences)
        {
            if (sentence.Contains("mythological") || sentence.Contains("legend") || sentence.Contains("sacred"))
            {
                return sentence.Trim();
            }
        }
        return "Significant mythological place";
    }

    private (decimal latitude, decimal longitude, string city, string state, string country) ExtractLocationFromContent(string content)
    {
        // This is a simplified extraction - in a real implementation, you'd use more sophisticated NLP
        // For now, return default coordinates for India
        return (28.6139m, 77.2090m, "Delhi", "Delhi", "India");
    }
}
