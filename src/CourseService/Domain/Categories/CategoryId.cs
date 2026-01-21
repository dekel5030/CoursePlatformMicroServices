namespace Courses.Domain.Categories;

public sealed record CategoryId(Guid Value)
{
    public static CategoryId CreateNew() => new(Guid.CreateVersion7());
    public override string ToString()
    {
        return Value.ToString();
    }
}
