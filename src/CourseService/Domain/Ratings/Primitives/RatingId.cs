namespace Courses.Domain.Ratings.Primitives;

public sealed record RatingId(Guid Value)
{
    public static RatingId New() => new(Guid.NewGuid());
}
