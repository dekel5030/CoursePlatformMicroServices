namespace Gateway.Api.Services.AuthClient;

internal interface IAuthClient
{
    Task<string?> GetInternalToken(string idpToken, CancellationToken cancellationToken = default);
}