using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourseSummaries;

internal sealed record GetCourseSummariesQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
