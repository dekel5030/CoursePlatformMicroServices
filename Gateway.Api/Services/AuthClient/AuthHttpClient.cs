using System.Net.Http.Headers;

namespace Gateway.Api.Services.AuthClient;

public record TokenResponse(string InternalToken);

public class AuthHttpClient : IAuthClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthHttpClient> _logger;

    public AuthHttpClient(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthHttpClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string?> GetInternalToken(string idpToken, CancellationToken cancellationToken = default)
    {
        var path = $"auth/exchange-token";

        try
        {
            var client = _httpClientFactory.CreateClient(DependencyInjection.AuthServiceName);
            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idpToken);

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) return null;

            var internalToken = await response.Content
               .ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

            return internalToken?.InternalToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch permissions from AuthService via HTTP");
            return null;
        }
    }
}