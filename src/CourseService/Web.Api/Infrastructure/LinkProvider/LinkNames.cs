namespace Courses.Api.Infrastructure.LinkProvider;

internal static class LinkNames
{
    public const string Self = "self";
    public const string Delete = "delete";
    public const string Create = "create";
    public const string Update = "partial-update";

    internal static class Courses
    {
        public const string Publish = "publish";
        public const string GenerateImageUploadUrl = "generate-image-upload-url";
        public const string CreateModule = "create-module";
    }

    internal static class Pagination
    {
        public const string NextPage = "next-page";
        public const string PreviousPage = "previous-page";
    }

    internal static class Modules
    {
        public const string CreateLesson = "create-lesson";
    }

    internal static class Lessons
    {
        public const string UploadVideoUrl = "upload-video-url";
        public const string AiGenerate = "ai-generate";
    }
}
