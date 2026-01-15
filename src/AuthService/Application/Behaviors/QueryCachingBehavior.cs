using Auth.Application.Abstractions.Caching;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Behaviors;

internal sealed class QueryCachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
    where TResponse : Result
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger;

    public QueryCachingBehavior(
        ICacheService cacheService, 
        ILogger<QueryCachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> nextHandler, 
        CancellationToken cancellationToken = default)
    {
        TResponse? cachedResult = await _cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);

        if (cachedResult != null)
        {
            _logger.LogInformation("Cache hit for query {QueryType} with key {CacheKey}.", typeof(TRequest).Name, request.CacheKey);

            return cachedResult;
        }

        _logger.LogInformation("Cache miss for query {QueryType} with key {CacheKey}.", typeof(TRequest).Name, request.CacheKey);

        TResponse result = await nextHandler();

        if (result.IsSuccess)
        {
            string cacheKey = request.CacheKey;

            await _cacheService.SetAsync(
                cacheKey,
                result,
                request.Expiration,
                cancellationToken);
        }

        return result;
    }
}
