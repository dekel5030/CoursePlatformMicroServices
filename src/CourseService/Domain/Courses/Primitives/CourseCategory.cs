namespace Courses.Domain.Courses.Primitives;

public sealed record CourseCategory
{
    public string Value { get; }

    private CourseCategory(string value)
    {
        Value = value;
    }

    public static readonly CourseCategory Programming = new("programming");
    public static readonly CourseCategory Design = new("design");
    public static readonly CourseCategory Architecture = new("architecture");
    public static readonly CourseCategory Unkown = new("Unkown");
}
