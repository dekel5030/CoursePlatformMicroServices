using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;

namespace Application.Courses.Queries.GetFeatured;

public record GetFeaturedQuery : IQuery<PagedResponseDto<CourseReadDto>>;
