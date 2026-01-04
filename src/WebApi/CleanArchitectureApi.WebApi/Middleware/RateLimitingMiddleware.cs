using System.Collections.Concurrent;
using System.Net;

namespace CleanArchitectureApi.WebApi.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;

        var clientInfo = _clients.AddOrUpdate(clientId, 
            new ClientRequestInfo { LastRequestTime = now, RequestCount = 1 },
            (key, existing) =>
            {
                if (now - existing.LastRequestTime > _options.Window)
                {
                    existing.RequestCount = 1;
                    existing.LastRequestTime = now;
                }
                else
                {
                    existing.RequestCount++;
                }
                return existing;
            });

        if (clientInfo.RequestCount > _options.MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers.Add("Retry-After", _options.Window.TotalSeconds.ToString());
            
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        // Clean up old entries periodically
        if (_clients.Count > 10000)
        {
            CleanupOldEntries(now);
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from JWT token first
        var userIdClaim = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            return $"user:{userIdClaim.Value}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }

    private void CleanupOldEntries(DateTime now)
    {
        var keysToRemove = _clients
            .Where(kvp => now - kvp.Value.LastRequestTime > _options.Window.Add(TimeSpan.FromMinutes(5)))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _clients.TryRemove(key, out _);
        }
    }
}

public class ClientRequestInfo
{
    public DateTime LastRequestTime { get; set; }
    public int RequestCount { get; set; }
}

public class RateLimitOptions
{
    public int MaxRequests { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}

