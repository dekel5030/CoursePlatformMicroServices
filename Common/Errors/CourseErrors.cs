namespace Common.Errors;

public static partial class CourseErrors
{
    public static readonly Error CourseNotFound = new("CourseNotFound", 404);
    public static readonly Error CourseAlreadyExists = new("CourseAlreadyExists", 409);
    public static readonly Error InvalidCourseData = new("InvalidCourseData", 400);
    public static readonly Error UnauthorizedAccess = new("UnauthorizedAccess", 403);
    public static readonly Error CourseCreationFailed = new("CourseCreationFailed", 500, IsPublic: false);

    public static readonly Error LessonNotFound = new("LessonNotFound", 404);
}