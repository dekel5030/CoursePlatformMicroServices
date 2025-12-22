using Kernel.Messaging.Abstractions;

namespace Application.Abstractions.Caching;

internal interface ICacheableCommand
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}

internal interface ICacheableCommand<TResponse> : ICommand<TResponse>, ICacheableCommand;
