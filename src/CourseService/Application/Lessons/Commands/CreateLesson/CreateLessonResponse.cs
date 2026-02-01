namespace Courses.Application.Lessons.Commands.CreateLesson;

public sealed record CreateLessonResponse(Guid LessonId, Guid CourseId, Guid ModuleId);
