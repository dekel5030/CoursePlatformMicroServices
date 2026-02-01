using Courses.Application.Enrollments.Dtos;
using Courses.Domain.Enrollments.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Queries.GetEnrollmentById;

public sealed record GetEnrollmentByIdQuery(EnrollmentId Id) : IQuery<EnrollmentDto>;
