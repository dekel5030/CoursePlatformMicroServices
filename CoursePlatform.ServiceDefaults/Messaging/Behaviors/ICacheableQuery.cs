using Kernel.Messaging.Abstractions;

namespace CoursePlatform.ServiceDefaults.Messaging.Behaviors;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}

public interface ICacheableQuery<TResponse> : IQuery<TResponse>, ICacheableQuery;