namespace Courses.Application.Services.LinkProvider.constants;

internal static class LinkRels
{
    public const string Self = "self";
    public const string Delete = "delete";
    public const string Create = "create";
    public const string PartialUpdate = "partial-update";

    internal static class Course
    {
        public const string GenerateImageUploadUrl = "generate-image-upload-url";
        public const string CreateModule = "create-module";
        public const string ReorderModules = "reorder-modules";
    }

    internal static class Lesson
    {
        public const string UploadVideoUrl = "upload-video-url";
        public const string AiGenerate = "ai-generate";
        public const string Move = "move";
    }

    internal static class Module
    {
        public const string CreateLesson = "create-lesson";
        public const string ReorderLessons = "reorder-lessons";
    }

    internal static class CourseRating
    {
        public const string Ratings = "ratings";
        public const string CreateRating = "create-rating";
    }

    internal static class Pagination
    {
        public const string NextPage = "next-page";
        public const string PreviousPage = "previous-page";
    }

    internal static class EnrolledCourse
    {
        public const string ViewCourse = "course";
        public const string ContinueLearning = "continue-learning";
    }
}
