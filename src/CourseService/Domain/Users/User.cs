using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared;

namespace Courses.Domain.Users;

public class User : IHasId<UserId>
{
    public required UserId Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl { get; set; }
}
