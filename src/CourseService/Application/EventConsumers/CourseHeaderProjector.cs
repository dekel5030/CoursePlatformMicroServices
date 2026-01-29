using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

internal sealed class CourseHeaderProjector :
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
    private readonly IWriteDbContext _writeDbContext;

    public CourseHeaderProjector(IReadDbContext readDbContext, IWriteDbContext writeDbContext)
    {
        _readDbContext = readDbContext;
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(
        CourseCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        // Fetch category details to denormalize
        var category = await _readDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new CategoryId(message.CategoryId), cancellationToken);

        var courseHeader = new CourseHeaderReadModel
        {
            Id = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            InstructorId = message.InstructorId,
            PriceAmount = message.PriceAmount,
            PriceCurrency = message.PriceCurrency,
            Status = Enum.Parse<CourseStatus>(message.Status),
            Language = message.Language,
            Difficulty = Enum.Parse<DifficultyLevel>(message.Difficulty),
            Slug = message.Slug,
            CategoryId = message.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            CategorySlug = category?.Slug.Value ?? string.Empty,
            UpdatedAtUtc = DateTime.UtcNow,
            ImageUrls = [],
            Tags = []
        };

        _readDbContext.CourseHeaders.Add(courseHeader);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        CourseTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Title = message.NewTitle,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDescriptionChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Description = message.NewDescription,
            cancellationToken);
    }

    public Task HandleAsync(
        CoursePriceChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header =>
            {
                header.PriceAmount = message.NewAmount;
                header.PriceCurrency = message.NewCurrency;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        CourseStatusChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Status = Enum.Parse<CourseStatus>(message.NewStatus),
            cancellationToken);
    }

    public async Task HandleAsync(
        CourseCategoryChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        Category? category = await _writeDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new CategoryId(message.NewCategoryId), cancellationToken);

        if (category is null)
        {
            return;
        }

        await UpdateCourseHeaderAsync(
            message.CourseId,
            header =>
            {
                header.CategoryId = message.NewCategoryId;
                header.CategoryName = category.Name;
                header.CategorySlug = category.Slug.Value;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDifficultyChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Difficulty = Enum.Parse<DifficultyLevel>(message.NewDifficulty),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseLanguageChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Language = message.NewLanguage,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseSlugChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Slug = message.NewSlug,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseTagsUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.Tags = message.NewTags,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseImageAddedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header =>
            {
                if (!header.ImageUrls.Contains(message.ImageUrl))
                {
                    header.ImageUrls.Add(message.ImageUrl);
                }
            },
            cancellationToken);
    }

    public Task HandleAsync(
        CourseImageRemovedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseHeaderAsync(
            message.CourseId,
            header => header.ImageUrls.Remove(message.ImageUrl),
            cancellationToken);
    }

    private async Task UpdateCourseHeaderAsync(
        Guid courseId,
        Action<CourseHeaderReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseHeaderReadModel? header = await _readDbContext.CourseHeaders
            .FindAsync([courseId], cancellationToken);

        if (header is not null)
        {
            updateAction(header);
            header.UpdatedAtUtc = DateTime.UtcNow;
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
