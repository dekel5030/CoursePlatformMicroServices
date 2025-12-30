using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    string? Title,
    string? Description,
    Guid? InstructorId) 
        : ICommand<CreateCourseResponse>;
