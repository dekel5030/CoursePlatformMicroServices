using Kernel;

namespace Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(
        string cacheKey, 
        CancellationToken cancellationToken);

    Task SetAsync<TResponse>(
        string cacheKey,
        TResponse result,
        TimeSpan? expiration,
        CancellationToken cancellationToken);
}
