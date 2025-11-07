using Kernel;

namespace Domain.Courses.Errors;

public static class CourseErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Course.NotFound",
        "Course Not Found.");
}
