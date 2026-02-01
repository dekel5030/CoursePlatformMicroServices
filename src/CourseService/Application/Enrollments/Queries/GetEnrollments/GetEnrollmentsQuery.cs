using Courses.Application.Enrollments.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Queries.GetEnrollments;

public sealed record GetEnrollmentsQuery(
    Guid? CourseId,
    Guid? StudentId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<EnrollmentCollectionDto>;
