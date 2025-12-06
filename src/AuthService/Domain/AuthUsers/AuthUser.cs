using Domain.AuthUsers.Events;
using Domain.Roles;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    private readonly List<Role> _roles = new();

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string UserName { get; private set; } = null!;
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private AuthUser() { }

    public static AuthUser Create(string email, string userName, Role initialRole)
    {
        var authUser = new AuthUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = string.IsNullOrEmpty(userName) ? email : userName
        };

        authUser._roles.Add(initialRole);

        authUser.Raise(new UserRegisteredDomainEvent(
            authUser.Id,
            email,
            DateTime.UtcNow));

        return authUser;
    }
}