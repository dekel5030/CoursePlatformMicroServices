using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Application.Shared.Dtos;
using ApplicationCreateCourseResponse = Courses.Application.Courses.Commands.CreateCourse.CreateCourseResponse;
using ApplicationCourseSummaryDto = Courses.Application.Courses.Queries.Dtos.CourseSummaryDto;
using ApplicationCourseDetailsDto = Courses.Application.Courses.Queries.Dtos.CourseDetailsDto;
using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Extensions;

internal static class CourseMappingExtensions
{
    public static CreateCourseResponse ToApiContract(this ApplicationCreateCourseResponse dto)
    {
        return new CreateCourseResponse(
            dto.CourseId.Value,
            dto.Title);
    }

    public static CourseSummaryResponse ToApiContract(this ApplicationCourseSummaryDto dto, LinkProvider linkProvider)
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
            linkProvider.CreateCourseLinks(dto.Id));
    }

    public static CourseDetailsResponse ToApiContract(this ApplicationCourseDetailsDto dto, LinkProvider linkProvider)
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
            linkProvider.CreateCourseLinks(dto.Id));
    }

    public static PagedResponse<CourseSummaryResponse> ToApiContract(
        this PagedResponseDto<ApplicationCourseSummaryDto> dto, LinkProvider linkProvider)
    {
        return new PagedResponse<CourseSummaryResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract(linkProvider)).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems
        };
    }
}
