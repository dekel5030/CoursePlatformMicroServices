using Courses.Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

public record GetFeaturedQuery : IQuery<PagedResponseDto<CourseReadDto>>;
