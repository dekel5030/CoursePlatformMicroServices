namespace Courses.Domain.MediaPackages.Primitives;

public sealed record MediaPackageId(Guid Value)
{
    public static MediaPackageId NewId() => new(Guid.CreateVersion7());
}
