using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourseCacheable;

internal sealed record GetCourseCacheableQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
