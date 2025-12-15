using Gateway.Api.Models;

namespace Gateway.Api.Services.PermissionsSource;

public class AuthHttpPermissionsSource : IPermissionsSource
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthHttpPermissionsSource> _logger;

    public AuthHttpPermissionsSource(
        IHttpClientFactory httpClientFactory, 
        ILogger<AuthHttpPermissionsSource> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<UserPermissionsDto?> FetchPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var safeUserId = Uri.EscapeDataString(userId);
        var path = $"api/internal/users/{safeUserId}/permissions";

        try
        {
            var client = _httpClientFactory.CreateClient(DependencyInjection.AuthServiceName);
            return await client
                .GetFromJsonAsync<UserPermissionsDto>(path, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch permissions from AuthService via HTTP");
            return null;
        }
    }
}