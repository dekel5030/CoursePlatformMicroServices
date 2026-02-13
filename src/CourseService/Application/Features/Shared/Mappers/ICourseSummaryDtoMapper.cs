using Courses.Application.Features.Dtos;
using Courses.Application.Features.Management;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Users;

namespace Courses.Application.Features.Shared.Mappers;

internal interface ICourseSummaryDtoMapper
{
    CourseSummaryDto MapToCatalogSummary(Course course, User? instructor, Category? category);

    ManagedCourseSummaryDto MapToManagedSummary(
        Course course,
        User? instructor,
        Category? category,
        ManagedCourseStatsDto stats);
}
