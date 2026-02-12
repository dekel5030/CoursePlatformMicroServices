using Courses.Domain.Ratings.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.CourseRatings.Commands.DeleteCourseRating;

public sealed record DeleteCourseRatingCommand(RatingId RatingId) : ICommand;
