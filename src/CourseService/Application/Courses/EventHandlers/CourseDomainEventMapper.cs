using CoursePlatform.Contracts.CourseService;
using Courses.Domain.Courses;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.EventHandlers;

internal sealed class CourseDomainEventMapper :
    IDomainEventHandler<CourseCreatedDomainEvent>,
    IDomainEventHandler<CourseTitleChangedDomainEvent>,
    IDomainEventHandler<CourseDescriptionChangedDomainEvent>,
    IDomainEventHandler<CoursePriceChangedDomainEvent>,
    IDomainEventHandler<CourseStatusChangedDomainEvent>,
    IDomainEventHandler<CourseDomainEvents>,
    IDomainEventHandler<CourseDifficultyChangedDomainEvent>,
    IDomainEventHandler<CourseLanguageChangedDomainEvent>,
    IDomainEventHandler<CourseSlugChangedDomainEvent>,
    IDomainEventHandler<CourseTagsUpdatedDomainEvent>,
    IDomainEventHandler<CourseImageAddedDomainEvent>,
    IDomainEventHandler<CourseImageRemovedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CourseDomainEventMapper(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(
        CourseCreatedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseCreatedIntegrationEvent(
                message.CourseId.Value,
                message.InstructorId.Value,
                message.Title.Value,
                message.Description.Value,
                message.Price.Amount,
                message.Price.Currency,
                message.Status.ToString(),
                message.Slug.Value,
                message.Difficulty.ToString(),
                message.Language.Code,
                message.CategoryId.Value),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseTitleChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseTitleChangedIntegrationEvent(
                message.Id.Value,
                message.NewTitle.Value),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDescriptionChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseDescriptionChangedIntegrationEvent(
                message.Id.Value,
                message.NewDescription.Value),
            cancellationToken);
    }

    public Task HandleAsync(
        CoursePriceChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CoursePriceChangedIntegrationEvent(
                message.Id.Value,
                message.NewPrice.Amount,
                message.NewPrice.Currency),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseStatusChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseStatusChangedIntegrationEvent(
                message.Id.Value,
                message.NewStatus.ToString()),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDomainEvents message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseCategoryChangedIntegrationEvent(
                message.Id.Value,
                message.NewCategoryId.Value),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDifficultyChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseDifficultyChangedIntegrationEvent(
                message.Id.Value,
                message.NewDifficulty.ToString()),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseLanguageChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseLanguageChangedIntegrationEvent(
                message.Id.Value,
                message.NewLanguage.Code),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseSlugChangedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseSlugChangedIntegrationEvent(
                message.Id.Value,
                message.NewSlug.Value),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseTagsUpdatedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseTagsUpdatedIntegrationEvent(
                message.Id.Value,
                message.NewTags.Select(tag => tag.Value).ToList()),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseImageAddedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseImageAddedIntegrationEvent(
                message.Id.Value,
                message.ImageUrl.Path),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseImageRemovedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(
            new CourseImageRemovedIntegrationEvent(
                message.Id.Value,
                message.ImageUrl.Path),
            cancellationToken);
    }
}
