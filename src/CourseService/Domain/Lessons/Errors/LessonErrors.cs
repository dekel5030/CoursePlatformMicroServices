using Kernel;

namespace Courses.Domain.Lessons.Errors;

public static class LessonErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Lesson.NotFound",
        "Lesson Not Found.");

    public static readonly Error NoRawResourcesForProcessing = Error.Validation(
        "Lesson.NoRawResources",
        "Lesson has no raw resources to process.");
}
