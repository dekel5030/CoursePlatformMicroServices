namespace Courses.Application.Services.Actions.Lessons;

public sealed record LessonAction
{
    public string Value { get; }
    private LessonAction(string value) => Value = value;

    public static readonly LessonAction Update = new("Edit");
    public static readonly LessonAction Delete = new("Delete");
    public static readonly LessonAction Read = new("Read");
    public static readonly LessonAction UploadVideoUrl = new("UploadVideo");
}
