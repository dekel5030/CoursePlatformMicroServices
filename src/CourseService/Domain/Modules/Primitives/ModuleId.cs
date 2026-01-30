namespace Courses.Domain.Modules.Primitives;

public sealed record ModuleId(Guid Value)
{
    public static ModuleId CreateNew() => new(Guid.CreateVersion7());
    public override string ToString()
    {
        return Value.ToString();
    }
}
