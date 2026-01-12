using System.Net.Http.Headers;
using System.Text.Json;

namespace Gateway.Api.Services.AuthClient;

internal sealed record TokenResponse(string InternalToken);

internal sealed class AuthHttpClient : IAuthClient
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
            HttpClient client = _httpClientFactory.CreateClient(DependencyInjection.AuthServiceName);
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idpToken);

            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            TokenResponse? internalToken = await response.Content
               .ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

            return internalToken?.InternalToken;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while exchanging token with AuthService");
            return null;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout occurred while communicating with AuthService");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "AuthService returned an invalid JSON response");
            return null;
        }
    }
}
