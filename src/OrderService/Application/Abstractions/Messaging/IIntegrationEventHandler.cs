using Kernel;

namespace Application.Abstractions.Messaging;

public interface IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    Task<Result> Handle(TEvent request, CancellationToken cancellationToken = default);
}

