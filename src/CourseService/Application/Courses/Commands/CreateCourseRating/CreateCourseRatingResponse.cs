namespace Courses.Application.Courses.Commands.CreateCourseRating;

public sealed record CreateCourseRatingResponse(
    Guid RatingId,
    Guid CourseId,
    int Score);
