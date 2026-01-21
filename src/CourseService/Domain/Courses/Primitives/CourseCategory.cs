namespace Courses.Domain.Courses.Primitives;

public readonly record struct CourseCategory
{
    public string Value { get; }

    private CourseCategory(string value)
    {
        Value = value;
    }

    public static readonly CourseCategory Programming = new("programming");
    public static readonly CourseCategory Design = new("design");
    public static readonly CourseCategory Architecture = new("architecture");
}
