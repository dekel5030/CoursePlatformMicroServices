using System.Globalization;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared;

namespace Courses.Domain.Users;

public class User : IHasId<UserId>
{
    public UserId Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? AvatarUrl { get; private set; }

    public User(UserId id, string firstName, string lastName, string email, string? avatarUrl)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AvatarUrl = avatarUrl;
    }

    public string FullName => FormatFullName();

    private string FormatFullName()
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        string first = textInfo.ToTitleCase(FirstName.ToLower());
        string last = textInfo.ToTitleCase(LastName.ToLower());

        return $"{first} {last}";
    }
}
