namespace Courses.Application.Services.LinkProvider.Contracts;

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
    }

    internal static class Lesson
    {
        public const string UploadVideoUrl = "upload-video-url";
        public const string AiGenerate = "ai-generate";
    }

    internal static class Module
    {
        public const string CreateLesson = "create-lesson";
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
}
