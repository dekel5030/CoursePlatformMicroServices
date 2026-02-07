namespace Users.Domain.Users.Primitives;

public record LecturerProfileId(Guid Value)
{
    public static LecturerProfileId New() => new(Guid.CreateVersion7());
}
