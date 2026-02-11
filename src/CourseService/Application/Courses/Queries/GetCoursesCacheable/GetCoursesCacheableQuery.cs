using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCoursesCacheable;

internal sealed record GetCoursesCacheableQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
