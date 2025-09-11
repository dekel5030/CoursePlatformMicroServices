using Domain.Users.Primitives;
using Kernel;

namespace Domain.Users;

public sealed class User : IVersionedEntity
{
    public UserId Id { get; set; }
    public ExternalUserId ExternalUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long EntityVersion { get; private set; }

    private User() { }

    public static User Create(ExternalUserId externalUserId, string email, string fullname, bool isActive)
    {
        return new User
        {
            Id = new UserId(Guid.CreateVersion7()),
            ExternalUserId = externalUserId,
            Email = email,
            Fullname = fullname,
            IsActive = isActive
        };
    }
}