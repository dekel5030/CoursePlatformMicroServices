using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings.Errors;
using Kernel;

namespace Courses.Domain.Ratings;

public sealed class CourseRating : RatingBase
{
    public CourseId CourseId { get; private set; }

    private CourseRating(CourseId courseId, UserId userId, int score, string? comment)
        : base(userId, score, comment)
    {
        CourseId = courseId;
    }

    public static Result<CourseRating> CreateRate(CourseId courseId, UserId userId, int score, string? comment)
    {
        if (score < 1 || score > 5)
        {
            return Result<CourseRating>.Failure(RatingErrors.InvalidScore);
        }

        if (comment != null && comment.Length > 500)
        {
            return Result<CourseRating>.Failure(RatingErrors.InvalidCommentLength);
        }

        var rating = new CourseRating(courseId, userId, score, comment);

        rating.Raise(new CourseRatingCreatedDomainEvent(
            rating.Id,
            courseId,
            score));

        return Result.Success(rating);
    }

    public Result Update(int newScore, string? comment)
    {
        if (Score == newScore && Comment == comment)
        {
            return Result.Success();
        }

        if (newScore < 1 || newScore > 5)
        {
            return Result<CourseRating>.Failure(RatingErrors.InvalidScore);
        }

        if (comment != null && comment.Length > 500)
        {
            return Result<CourseRating>.Failure(RatingErrors.InvalidCommentLength);
        }

        int oldScore = Score;
        base.InternalUpdate(newScore, comment);

        Raise(new CourseRatingUpdatedDomainEvent(
            Id,
            CourseId,
            oldScore,
            newScore));

        return Result.Success();
    }
}
