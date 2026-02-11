namespace Courses.Application.Services.LinkProvider.constants;

/// <summary>
/// Endpoint display names used for link generation. Must match the names used when mapping endpoints in Web.Api (e.g. WithName).
/// </summary>
internal static class EndpointNames
{
    // Courses
    public const string GetCourseById = "GetCourseById";
    public const string GetManagedCourseById = "GetManagedCourseById";
    public const string GetCourseAnalytics = "GetCourseAnalytics";
    public const string GetCourses = "GetCourses";
    public const string CreateCourse = "CreateCourse";
    public const string PatchCourse = "PatchCourse";
    public const string DeleteCourse = "DeleteCourse";
    public const string GenerateCourseImageUploadUrl = "GenerateCourseImageUploadUrl";

    // Modules
    public const string CreateModule = "CreateModule";
    public const string PatchModule = "PatchModule";
    public const string DeleteModule = "DeleteModule";
    public const string ReorderModules = "ReorderModules";
    public const string ReorderLessons = "ReorderLessons";

    // Lessons
    public const string GetLessonById = "GetLessonById";
    public const string CreateLesson = "CreateLesson";
    public const string PatchLesson = "PatchLesson";
    public const string DeleteLesson = "DeleteLesson";
    public const string GenerateLessonVideoUploadUrl = "GenerateLessonVideoUploadUrl";
    public const string GenerateLessonWithAi = "GenerateLessonWithAi";
    public const string MoveLesson = "MoveLesson";

    // Course ratings
    public const string GetCourseRatings = "GetCourseRatings";
    public const string CreateCourseRating = "CreateCourseRating";
    public const string UpdateCourseRating = "UpdateCourseRating";
    public const string DeleteCourseRating = "DeleteCourseRating";

    // Enrolled courses (current user)
    public const string GetEnrolledCourses = "GetEnrolledCourses";
}
