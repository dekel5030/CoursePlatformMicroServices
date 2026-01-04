using System.Diagnostics;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.Logging;

namespace CoursePlatform.ServiceDefaults.Messaging.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IUserContext _userContext;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        string? userIdRaw = _userContext.Id.ToString();
        string userId = string.IsNullOrEmpty(userIdRaw) ? "Anonymous" : userIdRaw;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["RequestType"] = requestName
        }))
        {
            _logger.LogInformation("Handling {RequestType} for User {UserId} with content: {@Request}",
                requestName, userId, request);

            var sw = Stopwatch.StartNew();
            try
            {
                TResponse response = await next();
                sw.Stop();

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Handled {RequestType} successfully in {Elapsed}ms",
                        requestName, sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogWarning("Handling {RequestType} failed in {Elapsed}ms. Error: {@Error}",
                        requestName, sw.ElapsedMilliseconds, response.Error);
                }

                return response;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Request {RequestType} for User {UserId} failed after {Elapsed}ms",
                    requestName, userId, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}