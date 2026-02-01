using Courses.Application.Enrollments.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Queries.GetEnrolledCourses;

public sealed record GetEnrolledCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10) : IQuery<EnrolledCourseCollectionDto>;
