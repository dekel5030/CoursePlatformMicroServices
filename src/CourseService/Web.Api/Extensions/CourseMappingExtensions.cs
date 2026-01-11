using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Application.Shared.Dtos;
using ApplicationCreateCourseResponse = Courses.Application.Courses.Commands.CreateCourse.CreateCourseResponse;
using ApplicationCourseSummaryDto = Courses.Application.Courses.Queries.Dtos.CourseSummaryDto;
using ApplicationCourseDetailsDto = Courses.Application.Courses.Queries.Dtos.CourseDetailsDto;

namespace Courses.Api.Extensions;

public static class CourseMappingExtensions
{
    public static CreateCourseResponse ToApiContract(this ApplicationCreateCourseResponse dto)
    {
        return new CreateCourseResponse(
            dto.CourseId.Value,
            dto.Title);
    }

    public static CourseSummaryResponse ToApiContract(this ApplicationCourseSummaryDto dto)
    {
        return new CourseSummaryResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.InstructorName,
            dto.Price,
            dto.Currency,
            dto.ThumbnailUrl?.ToString(),
            dto.LessonsCount,
            dto.EnrollmentCount);
    }

    public static CourseDetailsResponse ToApiContract(this ApplicationCourseDetailsDto dto)
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
            dto.Lessons.Select(lesson => lesson.ToApiContract()).ToList());
    }

    public static PagedResponse<CourseSummaryResponse> ToApiContract(
        this PagedResponseDto<ApplicationCourseSummaryDto> dto)
    {
        return new PagedResponse<CourseSummaryResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract()).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems
        };
    }
}
