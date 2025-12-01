using Domain.Users.Events;
using Domain.Users.Primitives;
using Kernel;
using SharedKernel;

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
        AuthUserId authUserId,
        string email,
        UserId? userId = null,
        FullName? fullName = null,
        PhoneNumber? phoneNumber = null,
        DateTime? dateOfBirth = null)
    {
        var user = new User
        {
            // Use provided userId if available, otherwise generate new one
            // When userId is provided from AuthService, it should match authUserId
            Id = userId ?? new UserId(Guid.CreateVersion7()),
            AuthUserId = authUserId,
            Email = email,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth
        };

        // Raise domain event for user profile creation
        user.Raise(new UserProfileCreatedDomainEvent(
            user.Id,
            user.AuthUserId,
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