namespace Courses.Application.Actions.Primitives;

public sealed record LessonAction
{

    public string Value { get; init; }

    private LessonAction(string value)
    {
        Value = value;
    }

    public static readonly LessonAction Update = new("Update");
    public static readonly LessonAction Delete = new("Delete");
    public static readonly LessonAction Create = new("Create");
    public static readonly LessonAction Read = new("Read");
    public static readonly LessonAction UploadVideoUrl = new("UploadVideoUrl");

}
