using Courses.Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetById;

public record GetCourseByIdQuery(Guid Id) : IQuery<CourseDetailsDto>;