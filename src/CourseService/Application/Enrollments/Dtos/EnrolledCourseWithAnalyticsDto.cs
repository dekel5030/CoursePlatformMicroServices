namespace Courses.Application.Enrollments.Dtos;

public sealed record EnrolledCourseWithAnalyticsDto(
    EnrolledCourseDto EnrolledCourse,
    EnrolledCourseAnalyticsDto Analytics
);
