using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface ICourseRatingRepository : IRepository<CourseRating, RatingId>;
