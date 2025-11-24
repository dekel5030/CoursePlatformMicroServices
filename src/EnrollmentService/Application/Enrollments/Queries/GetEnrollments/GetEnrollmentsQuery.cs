using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Queries.GetEnrollments;

public sealed record GetEnrollmentsQuery(
    int? UserId = null,
    int? CourseId = null,
    EnrollmentStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResponse<EnrollmentReadDto>>;
