using System.Security.Claims;
using Gateway.Api.Services.AuthSource;
using Gateway.Api.Services.CacheService;
using Kernel.Auth;

namespace Gateway.Api.Middleware;

public class UserEnrichmentMiddleware : IMiddleware
{
    private readonly ICacheService _cacheService;
    private readonly IAuthClient _authClient;

    public UserEnrichmentMiddleware(
        ICacheService cacheService, IAuthClient authClient)
    {
        _cacheService = cacheService;
        _authClient = authClient;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var identityUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(identityUserId))
        {
            await next(context);
            return;
        }

        var internalToken = await GetOrExchangeInternalTokenAsync(context, identityUserId);

        if (!string.IsNullOrEmpty(internalToken))
        {
            context.Request.Headers.Authorization = $"Bearer {internalToken}";
        }

        await next(context);
    }

    private async Task<string?> GetOrExchangeInternalTokenAsync(HttpContext context, string identityUserId)
    {
        var cachedToken = await _cacheService.GetAsync<string>(AuthCacheKeys.UserInternalJwt(identityUserId));
        if (!string.IsNullOrEmpty(cachedToken))
        {
            return cachedToken;
        }

        var keycloakToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(keycloakToken))
        {
            return null;
        }

        var internalToken = await _authClient.GetInternalToken(keycloakToken, context.RequestAborted);

        return internalToken;
    }
}
