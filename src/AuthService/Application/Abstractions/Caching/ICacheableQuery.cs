using Kernel.Messaging.Abstractions;

namespace Application.Abstractions.Caching;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}

public interface ICacheableQuery<TResponse> : IQuery<TResponse>, ICacheableQuery;