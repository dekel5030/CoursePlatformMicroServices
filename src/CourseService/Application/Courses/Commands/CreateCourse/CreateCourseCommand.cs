using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(CreateCourseDto Dto) : ICommand<CourseId>;
