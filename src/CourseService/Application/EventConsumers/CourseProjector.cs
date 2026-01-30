using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Courses.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

/// <summary>
/// Projector for CourseReadModel - maintains course aggregate projections.
/// Uses ONLY Integration Events (no WriteDbContext dependency).
/// </summary>
internal sealed class CourseProjector :
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
    IEventConsumer<ModuleCreatedIntegrationEvent>,
    IEventConsumer<ModuleDeletedIntegrationEvent>,
    IEventConsumer<LessonCreatedIntegrationEvent>,
    IEventConsumer<LessonMediaChangedIntegrationEvent>,
    IEventConsumer<LessonDeletedIntegrationEvent>,
    IEventConsumer<EnrollmentCreatedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CourseProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        CourseCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var course = new CourseReadModel
        {
            Id = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            Slug = message.Slug,
            Status = Enum.Parse<CourseStatus>(message.Status),
            Difficulty = Enum.Parse<DifficultyLevel>(message.Difficulty),
            Language = message.Language,
            PriceAmount = message.PriceAmount,
            PriceCurrency = message.PriceCurrency,
            InstructorId = message.InstructorId,
            CategoryId = message.CategoryId,
            ImageUrls = [],
            Tags = [],
            TotalModules = 0,
            TotalLessons = 0,
            TotalDurationSeconds = 0,
            EnrollmentCount = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _readDbContext.Courses.Add(course);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        CourseTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Title = message.NewTitle,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDescriptionChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Description = message.NewDescription,
            cancellationToken);
    }

    public Task HandleAsync(
        CoursePriceChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course =>
            {
                course.PriceAmount = message.NewAmount;
                course.PriceCurrency = message.NewCurrency;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        CourseStatusChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Status = Enum.Parse<CourseStatus>(message.NewStatus),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseCategoryChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.CategoryId = message.NewCategoryId,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseDifficultyChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Difficulty = Enum.Parse<DifficultyLevel>(message.NewDifficulty),
            cancellationToken);
    }

    public Task HandleAsync(
        CourseLanguageChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Language = message.NewLanguage,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseSlugChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Slug = message.NewSlug,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseTagsUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.Tags = message.NewTags,
            cancellationToken);
    }

    public Task HandleAsync(
        CourseImageAddedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course =>
            {
                if (!course.ImageUrls.Contains(message.ImageUrl))
                {
                    course.ImageUrls.Add(message.ImageUrl);
                }
            },
            cancellationToken);
    }

    // Stats Events

    public Task HandleAsync(
        ModuleCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.TotalModules++,
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.TotalModules = Math.Max(0, course.TotalModules - 1),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course =>
            {
                course.TotalLessons++;
                course.TotalDurationSeconds += message.Duration.TotalSeconds;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        // Need to fetch old duration from LessonReadModel to calculate diff
        return UpdateCourseDurationAsync(message.CourseId, message.Id, message.Duration, cancellationToken);
    }

    public Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.TotalLessons = Math.Max(0, course.TotalLessons - 1),
            cancellationToken);
    }

    public Task HandleAsync(
        EnrollmentCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateCourseAsync(
            message.CourseId,
            course => course.EnrollmentCount++,
            cancellationToken);
    }

    // Helpers

    private async Task UpdateCourseAsync(
        Guid courseId,
        Action<CourseReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseReadModel? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return;
        }

        updateAction(course);
        course.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateCourseDurationAsync(
        Guid courseId,
        Guid lessonId,
        TimeSpan newDuration,
        CancellationToken cancellationToken)
    {
        CourseReadModel? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        LessonReadModel? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (course is null || lesson is null)
        {
            return;
        }

        double durationDiff = newDuration.TotalSeconds - lesson.Duration.TotalSeconds;
        course.TotalDurationSeconds += durationDiff;
        course.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _readDbContext.SaveChangesAsync(cancellationToken);
    }
}
