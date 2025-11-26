using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Queries.GetEnrollmentById;

public sealed record GetEnrollmentByIdQuery(EnrollmentId Id) : IQuery<EnrollmentReadDto>;
