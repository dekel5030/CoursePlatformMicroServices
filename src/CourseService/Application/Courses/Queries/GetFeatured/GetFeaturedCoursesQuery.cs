using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;

namespace Application.Courses.Queries.GetFeatured;

public record GetFeaturedCoursesQuery : IQuery<IEnumerable<CourseReadDto>>;
