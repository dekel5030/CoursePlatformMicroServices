using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;

namespace Application.Enrollments.Queries.GetEnrollments;

public sealed record GetEnrollmentsQuery(
    GetEnrollmentsQueryParams Params) : IQuery<PagedResponse<EnrollmentReadDto>>;
