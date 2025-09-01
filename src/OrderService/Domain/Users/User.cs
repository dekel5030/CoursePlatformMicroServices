namespace Domain.Users;

public sealed class User
{
    public UserId Id { get; set; }
    public string ExternalUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    private User() { }

    public static User Create(string externalUserId, string email, string fullname, bool isActive)
    {
        return new User
        {
            Id = new UserId(Guid.NewGuid()),
            ExternalUserId = externalUserId,
            Email = email,
            Fullname = fullname,
            IsActive = isActive
        };
    }
}