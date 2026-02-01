using Courses.Application.Courses.Dtos;
using Courses.Domain.Ratings.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourseRatings;

public sealed record GetCourseRatingsQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<CourseRatingCollection>;
