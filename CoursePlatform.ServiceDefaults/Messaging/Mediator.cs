using System.Collections.Concurrent;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.ServiceDefaults.Messaging;

public sealed class Mediator : IMediator
{
    private static readonly ConcurrentDictionary<Type, object> _sendWrapperDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> _handlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> _wrapperTypeDictionary = new();
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

        var wrapper = (ISendHandlerWrapper<TResponse>)_sendWrapperDictionary.GetOrAdd(
            requestType,
            t => Activator.CreateInstance(typeof(SendHandlerWrapper<,>).MakeGenericType(t, typeof(TResponse)))!);

        return await wrapper.Handle(request, _serviceProvider, cancellationToken);
    }

    private interface ISendHandlerWrapper<TResponse>
    {
        Task<TResponse> Handle(object request, IServiceProvider sp, CancellationToken ct);
    }

    private sealed class SendHandlerWrapper<TRequest, TResponse> : ISendHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(object request, IServiceProvider sp, CancellationToken ct)
        {
            var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var behaviors = sp.GetServices<IPipelineBehavior<TRequest, TResponse>>();

            RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle((TRequest)request, ct);

            return await behaviors
                .Reverse()
                .Aggregate(handlerDelegate, (next, behavior) => () => behavior.Handle((TRequest)request, next, ct))();
        }
    }

    public async Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        //using IServiceScope scope = serviceProvider.CreateScope();

        Type domainEventType = domainEvent.GetType();
        Type handlerType = _handlerTypeDictionary.GetOrAdd(
            domainEventType,
            et => typeof(IDomainEventHandler<>).MakeGenericType(et));

        //IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(handlerType);
        IEnumerable<object?> handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is null)
            {
                continue;
            }

            var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);

            await handlerWrapper.Handle(domainEvent, cancellationToken);
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            Type wrapperType = _wrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));

            var instance = Activator.CreateInstance(wrapperType, handler)
                      ?? throw new InvalidOperationException(
                          $"Could not create wrapper '{wrapperType.FullName}' for handler '{handler.GetType().FullName}'.");

            return (HandlerWrapper)instance;
        }
    }

    private sealed class HandlerWrapper<T>(object handler) : HandlerWrapper where T : IDomainEvent
    {
        private readonly IDomainEventHandler<T> _handler = (IDomainEventHandler<T>)handler;

        public override async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _handler.Handle((T)domainEvent, cancellationToken);
        }
    }
}