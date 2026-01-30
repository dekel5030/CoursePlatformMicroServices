using CoursePlatform.Contracts.CourseService;
using Courses.Domain.Module;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.EventHandlers;

internal sealed class ModuleDomainEventMapper :
    IDomainEventHandler<ModuleCreatedDomainEvent>,
    IDomainEventHandler<ModuleTitleChangedDomainEvent>,
    IDomainEventHandler<ModuleIndexUpdatedDomainEvent>,
    IDomainEventHandler<ModuleDeletedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public ModuleDomainEventMapper(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(ModuleCreatedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new ModuleCreatedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.Title.Value,
            message.Index), cancellationToken);
    }

    public Task HandleAsync(ModuleTitleChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new ModuleTitleChangedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.NewTitle.Value), cancellationToken);
    }

    public Task HandleAsync(ModuleIndexUpdatedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new ModuleIndexUpdatedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.NewIndex), cancellationToken);
    }

    public Task HandleAsync(ModuleDeletedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new ModuleDeletedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value), cancellationToken);
    }
}