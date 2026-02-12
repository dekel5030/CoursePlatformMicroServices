using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.CourseRating.Queries.GetCourseRatings;

public sealed record GetCourseRatingsQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<CourseRatingCollection>;
