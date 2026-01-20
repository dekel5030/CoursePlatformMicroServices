namespace Courses.Domain.Courses.Primitives;

public record struct UserId(Guid Value)
{
    public static bool TryParse(string value, out UserId userId)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            userId = new UserId(guid);
            return true;
        }

        userId = default!;
        return false;
    }
};
