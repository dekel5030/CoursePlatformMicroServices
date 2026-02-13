using Courses.Application.Users.Dtos;
using Courses.Domain.Users;

namespace Courses.Application.Users;

public static class UserDtoMapper
{
    public static UserDto Map(User? user, Guid? fallbackId = null)
    {
        if (user is null)
        {
            return new UserDto
            {
                Id = fallbackId ?? Guid.Empty,
                FirstName = "Unknown",
                LastName = "Instructor",
                AvatarUrl = null
            };
        }

        return new UserDto
        {
            Id = user.Id.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUrl = user.AvatarUrl
        };
    }
}
