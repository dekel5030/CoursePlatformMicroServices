namespace Application.Abstractions.Messaging;

public interface IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    Task Handle(TEvent request, CancellationToken cancellationToken = default);
}