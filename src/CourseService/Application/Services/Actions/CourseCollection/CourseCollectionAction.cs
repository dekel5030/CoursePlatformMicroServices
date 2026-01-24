namespace Courses.Application.Services.Actions.CourseCollection;

public sealed record CourseCollectionAction
{
    public string Value { get; }
    private CourseCollectionAction(string value) => Value = value;

    public static readonly CourseCollectionAction CreateCourse = new("CreateCourse");
}
