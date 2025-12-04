using Domain.AuthUsers.Events;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string UserName { get; private set; } = null!;

    private AuthUser() { }

    public static AuthUser Create(string email, string? userName)
    {
        var authUser = new AuthUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = string.IsNullOrEmpty(userName) ? email : userName
        };

        authUser.Raise(new UserRegisteredDomainEvent(
            authUser.Id,
            email,
            DateTime.UtcNow));

        return authUser;
    }
}