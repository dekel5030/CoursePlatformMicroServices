namespace Courses.Application.CourseRatings.Commands.CreateCourseRating;

public sealed record CreateCourseRatingResponse(
    Guid RatingId,
    Guid CourseId,
    int Score);
