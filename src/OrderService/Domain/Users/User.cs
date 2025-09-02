using Domain.Users.Primitives;

namespace Domain.Users;

public sealed class User
{
    public UserId Id { get; set; }
    public ExternalUserId ExternalUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public bool IsActive { get; set; }

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