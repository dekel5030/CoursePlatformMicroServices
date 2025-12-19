namespace Gateway.Api.Services.CacheService;

public interface ICacheService
{
    Task<T?> GetAsync<T>(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SetAsync<TResponse>(
        string cacheKey,
        TResponse result,
        TimeSpan? expiration,
        CancellationToken cancellationToken = default);
}
