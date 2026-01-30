using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Shared.Extensions;

/// <summary>
/// Composition extensions for building DTOs from Core Read Models.
/// Enables flexible data shaping without Model Explosion.
/// </summary>
public static class ReadModelComposer
{
    #region Course Compositions

    /// <summary>
    /// Compose CourseListItem from CourseReadModel + InstructorReadModel + CategoryReadModel
    /// </summary>
    public static CourseSummaryDto ToCourseSummaryDto(
        this CourseReadModel course,
        InstructorReadModel? instructor,
        CategoryReadModel? category,
        string? resolvedThumbnailUrl = null)
    {
        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Slug = course.Slug ?? string.Empty,
            ShortDescription = course.Description.Length > 100
                ? course.Description[..100] + "..."
                : course.Description,
            Price = new Money(course.PriceAmount, course.PriceCurrency),
            OriginalPrice = new Money(course.PriceAmount, course.PriceCurrency),
            Difficulty = course.Difficulty,
            Status = course.Status,
            UpdatedAtUtc = course.UpdatedAtUtc,

            EnrollmentCount = course.EnrollmentCount,
            LessonsCount = course.TotalLessons,
            Duration = TimeSpan.FromSeconds(course.TotalDurationSeconds),

            AverageRating = 4.5, //  from reviews
            ReviewsCount = 120,  //  from reviews
            CourseViews = 1245,  //  from analytics

            Instructor = instructor != null
                ? new InstructorDto(instructor.Id, instructor.FullName, instructor.AvatarUrl)
                : new InstructorDto(course.InstructorId, "Unknown", null),

            Category = category != null
                ? new CategoryDto(category.Id, category.Name, category.Slug)
                : null!,

            ThumbnailUrl = resolvedThumbnailUrl,
            Badges = course.UpdatedAtUtc > DateTimeOffset.UtcNow.AddDays(-30)
                ? new List<string> { "New" }
                : [],

            Links = [] // Set by caller using LinkFactory
        };
    }

    #endregion

    #region Module Compositions

    /// <summary>
    /// Compose ModuleDetails from ModuleReadModel + List<LessonReadModel>
    /// </summary>
    public static ModuleDetailsDto ToModuleDetailsDto(
        this ModuleReadModel module,
        List<LessonReadModel> lessons)
    {
        var lessonSummaries = lessons
            .OrderBy(l => l.Index)
            .Select(l => new LessonSummaryDto(
                module.Id,
                l.Id,
                l.Title,
                l.Index,
                l.Duration,
                l.ThumbnailUrl,
                l.Access))
            .ToList();

        return new ModuleDetailsDto(
            module.Id,
            module.Title,
            module.Index,
            module.LessonCount,
            TimeSpan.FromSeconds(module.TotalDurationSeconds),
            lessonSummaries);
    }

    /// <summary>
    /// Compose ModuleSummary from ModuleReadModel (without lessons)
    /// </summary>
    public static ModuleSummaryDto ToModuleSummaryDto(this ModuleReadModel module)
    {
        return new ModuleSummaryDto(
            module.Id,
            module.Title,
            module.Index,
            module.LessonCount);
    }

    #endregion

    #region Lesson Compositions

    /// <summary>
    /// Compose LessonDetails from LessonReadModel
    /// </summary>
    public static LessonDetailsDto ToLessonDetailsDto(this LessonReadModel lesson)
    {
        return new LessonDetailsDto(
            lesson.ModuleId,
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Index,
            lesson.Duration,
            lesson.ThumbnailUrl,
            lesson.Access,
            lesson.VideoUrl);
    }

    /// <summary>
    /// Compose LessonSummary from LessonReadModel
    /// </summary>
    public static LessonSummaryDto ToLessonSummaryDto(this LessonReadModel lesson)
    {
        return new LessonSummaryDto(
            lesson.ModuleId,
            lesson.Id,
            lesson.Title,
            lesson.Index,
            lesson.Duration,
            lesson.ThumbnailUrl,
            lesson.Access);
    }

    #endregion

    #region Category Compositions

    public static CategoryDto ToCategoryDto(this CategoryReadModel category)
    {
        return new CategoryDto(category.Id, category.Name, category.Slug);
    }

    #endregion
}
