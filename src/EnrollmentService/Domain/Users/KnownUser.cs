using Domain.Users.Primitives;

namespace Domain.Users;

public class KnownUser
{
    private KnownUser() { }

    public ExternalUserId UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static KnownUser Create(ExternalUserId userId, string name, bool isActive)
    {
        return new KnownUser
        {
            UserId = userId,
            Name = name,
            IsActive = isActive
        };
    }

    public void Update(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
    }
}
