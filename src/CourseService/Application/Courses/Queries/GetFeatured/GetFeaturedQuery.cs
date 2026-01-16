using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

public record GetFeaturedQuery : IQuery<CourseCollectionDto>;
