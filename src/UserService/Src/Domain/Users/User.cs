using Domain.Users.Primitives;

namespace Domain.Users;

public class User
{
    public UserId Id { get; private set; }
    public string Email { get; private set; }
    public FullName FullName { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    private User() { }

    public static User CreateUser(string email, FullName fullName, PhoneNumber? phoneNumber, DateTime? dateOfBirth)
    {
        var user = new User
        {
            Id = new UserId(Guid.CreateVersion7()),
            Email = email,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth
        };

        // raise domain events here if needed

        return user;
    }
}
