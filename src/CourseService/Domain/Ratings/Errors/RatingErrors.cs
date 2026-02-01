using Kernel;

namespace Courses.Domain.Ratings.Errors;

public static class RatingErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "CourseRating.NotFound",
        "The specified course rating was not found.");

    public static readonly Error Unauthorized = Error.Unauthorized(
        "CourseRating.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error AlreadyRated = Error.Conflict(
        "CourseRating.AlreadyRated",
        "You have already rated this course.");

    public static readonly Error InvalidScore = Error.Validation(
        "CourseRating.InvalidScore",
        "Score must be between 1 and 5.");

    public static readonly Error InvalidCommentLength = Error.Validation(
        "CourseRating.InvalidCommentLength",
        "Comment cannot exceed 500 characters.");
}
