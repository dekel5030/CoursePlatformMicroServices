using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourseRating;

public sealed record CreateCourseRatingCommand(
    CourseId CourseId,
    int Score,
    string? Comment) : ICommand<CreateCourseRatingResponse>;
