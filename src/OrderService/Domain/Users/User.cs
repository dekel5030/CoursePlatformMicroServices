using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public UserId Id { get; set; }
    public required string ExternalUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
}