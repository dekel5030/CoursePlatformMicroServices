using Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(CreateCourseDto Dto) : ICommand<CourseId>;
