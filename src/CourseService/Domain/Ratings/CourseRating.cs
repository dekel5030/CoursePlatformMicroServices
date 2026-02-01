using Courses.Domain.Courses.Primitives;

namespace Courses.Domain.Ratings;

public sealed class CourseRating : RatingBase
{
    public CourseId CourseId { get; private set; }
    private CourseRating(CourseId courseId, UserId userId, int score, string? comment)
        : base(userId, score, comment)
    {
        CourseId = courseId;
    }

    public static CourseRating CreateRate(CourseId courseId, UserId userId, int score, string? comment)
    {
        var rating = new CourseRating(courseId, userId, score, comment);

        rating.Raise(new CourseRatingCreatedDomainEvent(
            rating.Id,
            courseId,
            score));

        return rating;
    }

    public void Update(int newScore, string? comment)
    {
        if (Score == newScore && Comment == comment)
        {
            return;
        }

        int oldScore = Score;
        base.InternalUpdate(newScore, comment);

        Raise(new CourseRatingUpdatedDomainEvent(
            Id,
            CourseId,
            oldScore,
            newScore));
    }
}
