namespace Courses.Application.Actions.Primitives;

public sealed record CourseAction
{
    public string Value { get; }
    private CourseAction(string value) => Value = value;

    public static readonly CourseAction Update = new("Edit");
    public static readonly CourseAction Publish = new("Publish");
    public static readonly CourseAction CreateLesson = new("CreateLesson");
    public static readonly CourseAction Delete = new("Delete");
    public static readonly CourseAction Read = new("Read");
    public static readonly CourseAction UploadImageUrl = new("UploadImage");
}
