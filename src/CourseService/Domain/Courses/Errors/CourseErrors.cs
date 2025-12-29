using Kernel;

namespace Courses.Domain.Courses.Errors;

public static class CourseErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Course.NotFound",
        "Course Not Found.");

    internal static readonly Error AlreadyPublished = Error.Conflict(
        "Course.AlreadyPublished",
        "Course Already Published.");

    internal static readonly Error CourseWithoutLessons = Error.Validation(
        "Course.WithoutLessons",
        "Course Could Not Be Published Without Lessons.");

    internal static readonly Error LessonAlreadyExists = Error.Validation(
        "Course.LessonAlreadyExists",
        "Course Lesson Already Exists.");
    internal static readonly Error InvalidPrice;
    internal static readonly Error ImageAlreadyExists;
    internal static readonly Error CourseNotPublished;
}
