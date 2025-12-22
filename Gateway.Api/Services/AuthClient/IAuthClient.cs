namespace Gateway.Api.Services.AuthClient;

public interface IAuthClient
{
    Task<string?> GetInternalToken(string idpToken, CancellationToken cancellationToken = default);
}