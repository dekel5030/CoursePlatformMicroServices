namespace Kernel.Messaging.Abstractions;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}