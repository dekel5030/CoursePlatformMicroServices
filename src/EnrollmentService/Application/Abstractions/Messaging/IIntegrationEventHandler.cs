namespace Application.Abstractions.Messaging;

public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
