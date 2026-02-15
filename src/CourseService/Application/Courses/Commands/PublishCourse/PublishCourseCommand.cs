using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PublishCourse;

public record PublishCourseCommand(CourseId CourseId) : ICommand;
