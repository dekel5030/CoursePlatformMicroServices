namespace Domain.Users;

public class User
{
    public UserId Id { get; private set; }
    public required string Email { get; set; }
    public required FullName FullName { get; set; }
    public PhoneNumber? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

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