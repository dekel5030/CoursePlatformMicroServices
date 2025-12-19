namespace Gateway.Api.Services.AuthSource;

public interface IAuthClient
{
    Task<string?> GetInternalToken(string idpToken, CancellationToken cancellationToken = default);
}