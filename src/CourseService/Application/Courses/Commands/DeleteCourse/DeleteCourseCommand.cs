using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(CourseId CourseId) : ICommand;