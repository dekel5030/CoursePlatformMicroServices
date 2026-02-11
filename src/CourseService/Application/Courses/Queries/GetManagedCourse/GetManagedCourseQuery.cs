using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetManagedCourse;

public sealed record GetManagedCourseQuery(Guid Id) : IQuery<ManagedCoursePageDto>;
