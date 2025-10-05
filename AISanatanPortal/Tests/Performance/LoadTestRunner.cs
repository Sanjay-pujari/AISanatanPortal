using System.Diagnostics;
using System.Text.Json;
using System.Text;
using AISanatanPortal.API.DTOs;

namespace AISanatanPortal.API.Tests.Performance;

public class LoadTestRunner
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public LoadTestRunner(string baseUrl = "https://localhost:7000")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<LoadTestResult> RunConcurrentRequestsTest(
        string endpoint, 
        int concurrentUsers, 
        int requestsPerUser,
        string? authToken = null)
    {
        var results = new List<RequestResult>();
        var stopwatch = Stopwatch.StartNew();
        
        // Create tasks for concurrent requests
        var tasks = new List<Task>();
        var semaphore = new SemaphoreSlim(concurrentUsers);
        
        for (int user = 0; user < concurrentUsers; user++)
        {
            tasks.Add(Task.Run(async () =>
            {
                for (int request = 0; request < requestsPerUser; request++)
                {
                    await semaphore.WaitAsync();
                    
                    try
                    {
                        var result = await MakeRequest(endpoint, authToken);
                        lock (results)
                        {
                            results.Add(result);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }));
        }
        
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        return new LoadTestResult
        {
            TotalRequests = results.Count,
            ConcurrentUsers = concurrentUsers,
            RequestsPerUser = requestsPerUser,
            TotalTime = stopwatch.Elapsed,
            AverageResponseTime = results.Average(r => r.ResponseTime),
            MinResponseTime = results.Min(r => r.ResponseTime),
            MaxResponseTime = results.Max(r => r.ResponseTime),
            SuccessCount = results.Count(r => r.IsSuccess),
            FailureCount = results.Count(r => !r.IsSuccess),
            Results = results
        };
    }

    public async Task<LoadTestResult> RunSustainedLoadTest(
        string endpoint,
        int durationInSeconds,
        int requestsPerSecond,
        string? authToken = null)
    {
        var results = new List<RequestResult>();
        var stopwatch = Stopwatch.StartNew();
        var interval = TimeSpan.FromMilliseconds(1000.0 / requestsPerSecond);
        var nextRequest = DateTime.UtcNow;
        
        while (stopwatch.Elapsed.TotalSeconds < durationInSeconds)
        {
            if (DateTime.UtcNow >= nextRequest)
            {
                var result = await MakeRequest(endpoint, authToken);
                results.Add(result);
                nextRequest = nextRequest.Add(interval);
            }
            
            await Task.Delay(10); // Small delay to prevent CPU spinning
        }
        
        stopwatch.Stop();
        
        return new LoadTestResult
        {
            TotalRequests = results.Count,
            TotalTime = stopwatch.Elapsed,
            AverageResponseTime = results.Average(r => r.ResponseTime),
            MinResponseTime = results.Min(r => r.ResponseTime),
            MaxResponseTime = results.Max(r => r.ResponseTime),
            SuccessCount = results.Count(r => r.IsSuccess),
            FailureCount = results.Count(r => !r.IsSuccess),
            Results = results
        };
    }

    private async Task<RequestResult> MakeRequest(string endpoint, string? authToken = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        
        if (!string.IsNullOrEmpty(authToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
        }
        
        try
        {
            var response = await _httpClient.SendAsync(request);
            stopwatch.Stop();
            
            var content = await response.Content.ReadAsStringAsync();
            
            return new RequestResult
            {
                ResponseTime = stopwatch.ElapsedMilliseconds,
                StatusCode = response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                ContentLength = content.Length,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            return new RequestResult
            {
                ResponseTime = stopwatch.ElapsedMilliseconds,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                IsSuccess = false,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<ApiLoadTestResult> RunAIAgentApiLoadTest(string? authToken = null)
    {
        var endpoints = new[]
        {
            "/api/aidataagent/status",
            "/api/aidataagent/progress",
            "/api/aidataagent/is-running"
        };
        
        var results = new Dictionary<string, LoadTestResult>();
        
        foreach (var endpoint in endpoints)
        {
            Console.WriteLine($"Testing endpoint: {endpoint}");
            var result = await RunConcurrentRequestsTest(endpoint, 10, 5, authToken);
            results[endpoint] = result;
            
            Console.WriteLine($"  - Total Requests: {result.TotalRequests}");
            Console.WriteLine($"  - Success Rate: {(double)result.SuccessCount / result.TotalRequests * 100:F2}%");
            Console.WriteLine($"  - Average Response Time: {result.AverageResponseTime:F2}ms");
            Console.WriteLine($"  - Max Response Time: {result.MaxResponseTime}ms");
        }
        
        return new ApiLoadTestResult
        {
            TestTimestamp = DateTime.UtcNow,
            EndpointResults = results,
            OverallSuccessRate = results.Values.Average(r => (double)r.SuccessCount / r.TotalRequests) * 100
        };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

public class LoadTestResult
{
    public int TotalRequests { get; set; }
    public int ConcurrentUsers { get; set; }
    public int RequestsPerUser { get; set; }
    public TimeSpan TotalTime { get; set; }
    public double AverageResponseTime { get; set; }
    public long MinResponseTime { get; set; }
    public long MaxResponseTime { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<RequestResult> Results { get; set; } = new();
    
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessCount / TotalRequests * 100 : 0;
    public double RequestsPerSecond => TotalTime.TotalSeconds > 0 ? TotalRequests / TotalTime.TotalSeconds : 0;
}

public class RequestResult
{
    public long ResponseTime { get; set; }
    public System.Net.HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public int ContentLength { get; set; }
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ApiLoadTestResult
{
    public DateTime TestTimestamp { get; set; }
    public Dictionary<string, LoadTestResult> EndpointResults { get; set; } = new();
    public double OverallSuccessRate { get; set; }
}


