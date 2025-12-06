using SharedKernel;

namespace Domain.Permissions;

public class Permission : Entity
{
    private Permission() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public static Permission Create(string name)
    {
        return new Permission
        {
            Id = Guid.CreateVersion7(),
            Name = name
        };
    }
}
