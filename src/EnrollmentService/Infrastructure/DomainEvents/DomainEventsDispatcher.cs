using Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Infrastructure.DomainEvents;

public class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventsDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IEnumerable<Entity> entities, CancellationToken cancellationToken = default)
    {
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await PublishDomainEvent(domainEvent, cancellationToken);
        }
    }

    private async Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod is not null)
            {
                await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
