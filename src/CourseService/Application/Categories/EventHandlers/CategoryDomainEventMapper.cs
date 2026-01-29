using CoursePlatform.Contracts.CourseService;
using Courses.Domain.Categories;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.EventHandlers;

internal sealed class CategoryDomainEventMapper :
    IDomainEventHandler<CategoryCreatedDomainEvent>,
    IDomainEventHandler<CategoryRenamedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CategoryDomainEventMapper(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(CategoryCreatedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new CategoryCreatedIntegrationEvent(
            message.Id.Value,
            message.Name,
            message.Slug.Value), cancellationToken);
    }

    public Task HandleAsync(CategoryRenamedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new CategoryRenamedIntegrationEvent(
            message.Id.Value,
            message.NewName,
            message.NewSlug.Value), cancellationToken);
    }
}