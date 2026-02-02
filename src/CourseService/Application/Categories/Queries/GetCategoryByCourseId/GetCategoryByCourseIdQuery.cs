using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategoryByCourseId;

public sealed record GetCategoryByCourseIdQuery(CourseId CourseId) : IQuery<CategoryDto>;
