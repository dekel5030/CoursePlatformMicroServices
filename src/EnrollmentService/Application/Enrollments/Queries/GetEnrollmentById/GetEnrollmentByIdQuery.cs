using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;

namespace Application.Enrollments.Queries.GetEnrollmentById;

public sealed record GetEnrollmentByIdQuery(Guid EnrollmentId) : IQuery<EnrollmentReadDto>;
