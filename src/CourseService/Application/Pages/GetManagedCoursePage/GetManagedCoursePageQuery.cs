using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Pages.GetManagedCoursePage;

public sealed record GetManagedCoursePageQuery(Guid Id) : IQuery<ManagedCoursePageDto>;
