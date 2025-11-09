using Application.Abstractions.Messaging;
using Domain.Courses.Primitives;
using SharedKernel;

namespace Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(CreateCourseDto Dto) : ICommand<CourseId>;
