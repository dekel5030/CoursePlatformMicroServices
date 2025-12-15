using Auth.Contracts.Dtos;
using Gateway.Api.Models;

namespace Gateway.Api.Services.AuthSource;

public class AuthHttpAdapter : IAuthSourceAdapter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthHttpAdapter> _logger;

    public AuthHttpAdapter(
        IHttpClientFactory httpClientFactory, 
        ILogger<AuthHttpAdapter> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<UserEnrichmentModel?> FetchPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var safeUserId = Uri.EscapeDataString(userId);
        var path = $"api/internal/users/{safeUserId}/auth-data";

        try
        {
            var client = _httpClientFactory.CreateClient(DependencyInjection.AuthServiceName);
            UserAuthDataDto? contract = await client
                .GetFromJsonAsync<UserAuthDataDto>(path, cancellationToken);

            if (contract == null) return null;

            return new UserEnrichmentModel
            {
                UserId = contract.UserId,
                Permissions = contract.Permissions,
                Roles = contract.Roles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch permissions from AuthService via HTTP");
            return null;
        }
    }
}