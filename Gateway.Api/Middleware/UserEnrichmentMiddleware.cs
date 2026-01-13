using System.Collections.Concurrent;
using System.Security.Claims;
using Gateway.Api.Services.AuthClient;
using Gateway.Api.Services.CacheService;
using Kernel.Auth;

namespace Gateway.Api.Middleware;

internal sealed class UserEnrichmentMiddleware : IMiddleware
{
    private readonly ICacheService _cacheService;
    private readonly IAuthClient _authClient;
    private readonly ILogger<UserEnrichmentMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, Task<string?>> _exchangeTasks = new();

    public UserEnrichmentMiddleware(
        ICacheService cacheService, IAuthClient authClient, ILogger<UserEnrichmentMiddleware> logger)
    {
        _cacheService = cacheService;
        _authClient = authClient;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        string? identityUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(identityUserId))
        {
            await next(context);
            return;
        }

        string? internalToken = await _exchangeTasks.GetOrAdd(identityUserId, (key) =>
            GetOrExchangeInternalTokenWithCleanupAsync(context, key));

        if (string.IsNullOrEmpty(internalToken))
        {
            _logger.LogWarning("Blocking request for user {UserId} - Internal token exchange failed", identityUserId);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Request.Headers.Authorization = $"Bearer {internalToken}";
        await next(context);
    }

    private async Task<string?> GetOrExchangeInternalTokenWithCleanupAsync(HttpContext context, string identityUserId)
    {
        try
        {
            return await GetOrExchangeInternalTokenAsync(context, identityUserId);
        }
        finally
        {
            _exchangeTasks.TryRemove(identityUserId, out _);
        }
    }

    private async Task<string?> GetOrExchangeInternalTokenAsync(HttpContext context, string identityUserId)
    {
        string? cachedToken = await _cacheService.GetAsync<string>(AuthCacheKeys.UserInternalJwt(identityUserId));
        if (!string.IsNullOrEmpty(cachedToken))
        {
            _logger.LogInformation("Cache hit for user {UserId}", identityUserId);
            return cachedToken;
        }

        string keycloakToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(keycloakToken))
        {
            return null;
        }

        _logger.LogInformation("Cache miss - Fetching internal token for user {UserId}", identityUserId);
        string? internalToken = await _authClient.GetInternalToken(keycloakToken);

        return internalToken;
    }
}
