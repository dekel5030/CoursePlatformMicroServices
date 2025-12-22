using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.ServiceDefaults.Messaging;

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        Type requestType = request.GetType();

        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException(
                $"Handler not found for request type {requestType.Name}.");
        }

        RequestHandlerDelegate<TResponse> executionPlan = () =>
        {
            var result = ((dynamic)handler).Handle((dynamic)request, cancellationToken);
            return (Task<TResponse>)result!;
        };

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        var behaviors = _serviceProvider.GetServices(behaviorType);

        foreach (var behavior in behaviors.Reverse())
        {
            if (behavior is null) continue;

            RequestHandlerDelegate<TResponse> next = executionPlan;

            executionPlan = () =>
            {
                var result = ((dynamic)behavior).Handle((dynamic)request, next, cancellationToken);
                return (Task<TResponse>)result!;
            };
        }

        return await executionPlan();
    }
}