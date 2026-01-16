using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Actions.Primitives;
using Courses.Application.Courses.Commands.CreateCourse;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Shared.Dtos;

namespace Courses.Api.Extensions;

internal static class CourseMappingExtensions
{
    public static CreateCourseResponse ToApiContract(this CreateCourseDto dto)
    {
        return new CreateCourseResponse(
            dto.CourseId.Value,
            dto.Title);
    }

    public static CourseSummaryResponse ToApiContract(
        this CourseSummaryDto dto,
        LinkProvider linkProvider)
    {
        return new CourseSummaryResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.InstructorName,
            dto.Price,
            dto.Currency,
            dto.ThumbnailUrl?.ToString(),
            dto.LessonsCount,
            dto.EnrollmentCount,
            linkProvider.CreateCourseLinks(dto.Id, dto.AllowedActions));
    }

    public static CourseDetailsResponse ToApiContract(this CourseDetailsDto dto, LinkProvider linkProvider)
    {
        return new CourseDetailsResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.InstructorName,
            dto.Price,
            dto.Currency,
            dto.EnrollmentCount,
            dto.UpdatedAtUtc,
            dto.ImageUrls.Select(url => url.ToString()).ToList(),
            dto.Lessons.Select(lesson => lesson.ToApiContract(dto.Id, linkProvider)).ToList(),
            linkProvider.CreateCourseLinks(dto.Id, dto.AllowedActions));
    }

    public static PagedResponse<CourseSummaryResponse> ToApiContract(
        this CourseCollectionDto dto,
        LinkProvider linkProvider,
        PagedQueryDto pagedQuery)
    {
        return new PagedResponse<CourseSummaryResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract(linkProvider)).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = linkProvider.CreateCourseCollectionLinks(dto, pagedQuery)
        };
    }
}
