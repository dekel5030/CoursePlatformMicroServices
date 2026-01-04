using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(Guid CourseId) : ICommand;