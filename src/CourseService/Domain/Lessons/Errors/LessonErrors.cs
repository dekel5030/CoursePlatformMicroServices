using Kernel;

namespace Domain.Lessons.Errors;

public static class LessonErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Lesson.NotFound",
        "Lesson Not Found.");
}
