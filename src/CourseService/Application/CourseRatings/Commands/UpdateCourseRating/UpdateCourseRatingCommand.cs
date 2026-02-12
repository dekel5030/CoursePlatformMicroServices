using Courses.Domain.Ratings.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.CourseRatings.Commands.UpdateCourseRating;

public sealed record UpdateCourseRatingCommand(
    RatingId RatingId,
    int? Score,
    string? Comment) : ICommand;
