using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Infrastructure.Database.Write.Repositories;

internal sealed class CourseRatingRepository : RepositoryBase<CourseRating, RatingId>, ICourseRatingRepository
{
    public CourseRatingRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
