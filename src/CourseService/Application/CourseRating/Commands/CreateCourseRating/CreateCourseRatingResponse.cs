namespace Courses.Application.CourseRating.Commands.CreateCourseRating;

public sealed record CreateCourseRatingResponse(
    Guid RatingId,
    Guid CourseId,
    int Score);
