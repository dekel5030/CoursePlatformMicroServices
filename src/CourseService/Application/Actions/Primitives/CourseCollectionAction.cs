namespace Courses.Application.Actions.Primitives;

public sealed record CourseCollectionAction
{
    public string Value { get; }
    private CourseCollectionAction(string value) => Value = value;

    public static readonly CourseCollectionAction CreateCourse = new("CreateCourse");
}
