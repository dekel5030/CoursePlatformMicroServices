using Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Application.Courses.Queries.GetFeatured;

public record GetFeaturedQuery : IQuery<PagedResponseDto<CourseReadDto>>;
