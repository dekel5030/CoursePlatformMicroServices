using Auth.Contracts.Dtos;
using CoursePlatform.ServiceDefaults.Auth;
using Gateway.Api.Models;

namespace Gateway.Api.Services.AuthSource;

public class AuthHttpAdapter : IAuthSourceAdapter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthHttpAdapter> _logger;

    public AuthHttpAdapter(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthHttpAdapter> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UserEnrichmentModel?> FetchPermissionsAsync(
        string identityUserId,
        CancellationToken cancellationToken = default)
    {
        var safeUserId = Uri.EscapeDataString(identityUserId);
        var path = $"internal/provision/{safeUserId}";

        try
        {
            var client = _httpClientFactory.CreateClient(DependencyInjection.AuthServiceName);

            var request = new HttpRequestMessage(HttpMethod.Post, path);

            ForwardHeader(HeaderNames.UserId, request);
            ForwardHeader(HeaderNames.FirstName, request);
            ForwardHeader(HeaderNames.LastName, request);
            ForwardHeader(HeaderNames.Email, request);

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) return null;

            UserAuthDataDto? contract = await response.Content
                .ReadFromJsonAsync<UserAuthDataDto>(cancellationToken: cancellationToken);

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

    private void ForwardHeader(string headerName, HttpRequestMessage request)
    {
        var currentRequest = _httpContextAccessor.HttpContext?.Request;

        if (currentRequest != null && currentRequest.Headers.TryGetValue(headerName, out var value) && !string.IsNullOrEmpty(value))
        {
            request.Headers.TryAddWithoutValidation(headerName, value.ToString());
        }
    }
}