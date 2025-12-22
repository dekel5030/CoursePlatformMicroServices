namespace Kernel.Messaging.Abstractions;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}