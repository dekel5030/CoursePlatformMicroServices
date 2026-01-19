using Kernel;
using Users.Domain.Users.Events;
using Users.Domain.Users.Primitives;

namespace Users.Domain.Users;

public class User : Entity
{
    public UserId Id { get; private set; }
#pragma warning disable S1144 // Unused private types or members should be removed
    public AuthUserId? AuthUserId { get; private set; }
#pragma warning restore S1144 // Unused private types or members should be removed
    public string Email { get; private set; } = null!;
    public FullName? FullName { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? AvatarUrl { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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
        user.Raise(new UserProfileCreatedDomainEvent(user));

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
