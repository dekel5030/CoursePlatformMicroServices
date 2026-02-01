using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings.Primitives;
using Courses.Domain.Shared;

namespace Courses.Domain.Ratings;

public abstract class RatingBase : Entity<RatingId>, IAuditable
{
    public override RatingId Id { get; protected set; } = RatingId.New();
    public int Score { get; private set; }
    public string? Comment { get; private set; }
    public UserId UserId { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected RatingBase(UserId userId, int score, string? comment)
    {
        UserId = userId;
        Score = score;
        Comment = comment;
    }

    protected void InternalUpdate(int score, string? comment)
    {
        Score = score;
        Comment = comment;
    }
}
