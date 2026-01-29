using CoursePlatform.Contracts.CourseEvents;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

internal sealed class CoursePageProjector :
    IEventConsumer<CourseCreatedIntegrationEvent>,
    IEventConsumer<CourseTitleChangedIntegrationEvent>,
    IEventConsumer<CourseDescriptionChangedIntegrationEvent>,
    IEventConsumer<CoursePriceChangedIntegrationEvent>,
    IEventConsumer<CourseStatusChangedIntegrationEvent>,
    IEventConsumer<CourseCategoryChangedIntegrationEvent>,
    IEventConsumer<CourseDifficultyChangedIntegrationEvent>,
    IEventConsumer<CourseLanguageChangedIntegrationEvent>,
    IEventConsumer<CourseSlugChangedIntegrationEvent>,
    IEventConsumer<CourseTagsUpdatedIntegrationEvent>,
    IEventConsumer<CourseImageAddedIntegrationEvent>,
    IEventConsumer<CourseImageRemovedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CoursePageProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        CourseCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var coursePage = new CoursePage
        {
            Id = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            InstructorId = message.InstructorId,
            PriceAmount = message.PriceAmount,
            PriceCurrency = message.PriceCurrency,
            Status = Enum.Parse<CourseStatus>(message.Status),
            UpdatedAtUtc = DateTime.UtcNow,
            Language = message.Language,
            Difficulty = Enum.Parse<DifficultyLevel>(message.Difficulty),
            Slug = message.Slug,
            CategoryId = message.CategoryId,
            ImageUrls = new List<string>(),
            Tags = new List<string>(),
            Modules = new List<ModuleReadModel>()
        };

        _readDbContext.CoursePages.Add(coursePage);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        CourseTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Title = message.NewTitle,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDescriptionChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Description = message.NewDescription,
            cancellationToken);
    }

    public Task HandleAsync(
        CoursePriceChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => { 
                coursePage.PriceAmount = message.NewAmount; 
                coursePage.PriceCurrency = message.NewCurrency; },
            cancellationToken);
    }

    public Task HandleAsync(
        CourseStatusChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Status = Enum.Parse<CourseStatus>(message.NewStatus),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseCategoryChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.CategoryId = message.NewCategoryId,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDifficultyChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Difficulty = Enum.Parse<DifficultyLevel>(message.NewDifficulty),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseLanguageChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Language = message.NewLanguage,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseSlugChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Slug = message.NewSlug,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseTagsUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.Tags = message.NewTags,
            cancellationToken);
    }

    public async Task HandleAsync(
        CourseImageAddedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        await UpdateCourseAsync(
            message.CourseId,
            coursePage => { 
                if (!coursePage.ImageUrls.Contains(message.ImageUrl))
                { 
                    coursePage.ImageUrls.Add(message.ImageUrl); 
                }
            },
            cancellationToken);
    }

    public async Task HandleAsync(
        CourseImageRemovedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        await UpdateCourseAsync(
            message.CourseId,
            coursePage => coursePage.ImageUrls.Remove(message.ImageUrl),
            cancellationToken);
    }

    private async Task UpdateCourseAsync(
        Guid courseId,
        Action<CoursePage> updateAction,
        CancellationToken cancellationToken)
    {
        CoursePage? page = await _readDbContext.CoursePages
            .FirstOrDefaultAsync(p => p.Id == courseId, cancellationToken);

        if (page is not null)
        {
            updateAction(page);
            page.UpdatedAtUtc = DateTime.UtcNow;
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
