using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeaturedCourseSummaries;

internal sealed record GetFeaturedCourseSummariesQuery : IQuery<CourseCollectionDto>;
