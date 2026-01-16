namespace Courses.Application.Actions.Primitives;

public sealed record LessonAction
{
    public string Value { get; init; }

    private LessonAction(string value)
    {
        Value = value;
    }

    public static LessonAction Update => new("Update");
    public static LessonAction Delete => new("Delete");
    public static LessonAction Create => new("Create");
    public static LessonAction Read => new("Read");
}
