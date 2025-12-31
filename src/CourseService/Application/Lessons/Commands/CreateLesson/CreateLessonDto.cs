namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonDto(
    Guid CourseId,
    string? Title,
    string? Description
);
