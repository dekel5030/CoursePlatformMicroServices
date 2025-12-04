using SharedKernel;

namespace Domain.Roles;

public class Role : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Role() { }

    public static Role Create(string roleName)
    {
        return new Role
        {
            Id = Guid.CreateVersion7(),
            Name = roleName,
        };
    }
}