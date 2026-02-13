using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Management;
using Courses.Application.Features.Shared;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Users;

namespace Courses.Application.Features.Shared.Mappers;

internal sealed class CourseSummaryDtoMapper : ICourseSummaryDtoMapper
{
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILinkBuilderService _linkBuilder;

    public CourseSummaryDtoMapper(
        IStorageUrlResolver urlResolver,
        ILinkBuilderService linkBuilder)
    {
        _urlResolver = urlResolver;
        _linkBuilder = linkBuilder;
    }

    public CourseSummaryDto MapToCatalogSummary(Course course, User? instructor, Category? category)
    {
        string? thumbnailUrl = CourseSummaryHelpers.GetFirstImagePublicUrl(course.Images, _urlResolver);
        string shortDescription = CourseSummaryHelpers.TruncateShortDescription(course.Description.Value);

        return new CourseSummaryDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            ShortDescription = shortDescription,
            Slug = course.Slug.Value,
            ThumbnailUrl = thumbnailUrl,
            Instructor = UserDtoMapper.Map(instructor),
            Category = CategoryDtoMapper.Map(category),
            Difficulty = course.Difficulty,
            Price = course.Price,
            UpdatedAtUtc = course.UpdatedAtUtc,
            Status = course.Status,
            Links = []
        };
    }

    public ManagedCourseSummaryDto MapToManagedSummary(
        Course course,
        User? instructor,
        Category? category,
        ManagedCourseStatsDto stats)
    {
        string? thumbnailUrl = CourseSummaryHelpers.GetFirstImagePublicUrl(course.Images, _urlResolver);
        string shortDescription = CourseSummaryHelpers.TruncateShortDescription(course.Description.Value);

        var courseContext = new CourseContext(course.Id, course.InstructorId, course.Status, IsManagementView: true);
        var links = _linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext).ToList();

        return new ManagedCourseSummaryDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            ShortDescription = shortDescription,
            Slug = course.Slug.Value,
            ThumbnailUrl = thumbnailUrl,
            Instructor = UserDtoMapper.Map(instructor, course.InstructorId.Value),
            Category = CategoryDtoMapper.Map(category),
            Difficulty = course.Difficulty,
            Price = course.Price,
            UpdatedAtUtc = course.UpdatedAtUtc,
            Status = course.Status,
            Stats = stats,
            Links = links
        };
    }
}
