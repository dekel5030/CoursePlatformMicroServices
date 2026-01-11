using Domain.Users.Events;
using Domain.Users.Primitives;
using Kernel;

namespace Domain.Users;

public class User : Entity
{
    public UserId Id { get; private set; }
    public AuthUserId? AuthUserId { get; private set; }
    public string Email { get; private set; } = null!;
    public FullName? FullName { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    private User() { }

    public static Result<User> CreateUser(
        UserId id,
        string? email,
        FullName? fullName = null,
        PhoneNumber? phoneNumber = null,
        DateTime? dateOfBirth = null)
    {
        var user = new User
        {
            Id = id,
            Email = email ?? string.Empty,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth
        };

        // Raise domain event for user profile creation
        user.Raise(new UserProfileCreatedDomainEvent(
            user.Id,
            user.AuthUserId!,
            user.Email,
            DateTime.UtcNow));

        return Result.Success(user);
    }

    public Result UpdateProfile(
        FullName? fullName = null,
        PhoneNumber? phoneNumber = null,
        DateTime? dateOfBirth = null)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;

        return Result.Success();
    }
}