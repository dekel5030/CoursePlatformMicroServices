using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Features.StudentDashboard.GetEnrolledCourses;

public sealed record GetEnrolledCoursesDto(
    IReadOnlyList<EnrolledCourseItemDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    GetEnrolledCoursesCollectionLinks Links);

public sealed record EnrolledCourseItemDto(
    EnrolledCourseItemData Data,
    EnrolledCourseLinks Links);

public sealed record EnrolledCourseItemData(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string? CourseImageUrl,
    string CourseSlug,
    DateTimeOffset? LastAccessedAt,
    DateTimeOffset EnrolledAt,
    string Status,
    double ProgressPercentage);

public sealed record EnrolledCourseLinks(
    LinkRecord ViewCourse,
    LinkRecord? ContinueLearning);

public sealed record GetEnrolledCoursesCollectionLinks(LinkRecord Self);
