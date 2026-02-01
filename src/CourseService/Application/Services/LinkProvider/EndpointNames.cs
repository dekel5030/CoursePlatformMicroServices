namespace Courses.Application.Services.LinkProvider;

/// <summary>
/// Endpoint display names used for link generation. Must match the names used when mapping endpoints in Web.Api (e.g. WithName).
/// </summary>
internal static class EndpointNames
{
    // Courses
    public const string GetCourseById = "GetCourseById";
    public const string GetCourses = "GetCourses";
    public const string CreateCourse = "CreateCourse";
    public const string PatchCourse = "PatchCourse";
    public const string DeleteCourse = "DeleteCourse";
    public const string GenerateCourseImageUploadUrl = "GenerateCourseImageUploadUrl";

    // Modules
    public const string CreateModule = "CreateModule";

    // Lessons
    public const string GetLessonById = "GetLessonById";
    public const string CreateLesson = "CreateLesson";
    public const string PatchLesson = "PatchLesson";
    public const string DeleteLesson = "DeleteLesson";
    public const string GenerateLessonVideoUploadUrl = "GenerateLessonVideoUploadUrl";
    public const string GenerateLessonWithAi = "GenerateLessonWithAi";
}
