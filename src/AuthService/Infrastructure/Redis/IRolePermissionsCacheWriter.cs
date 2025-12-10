namespace Infrastructure.Redis;
public interface IRolePermissionsCacheWriter
{
    Task UpdateAsync(
        string roleName,
        IReadOnlyCollection<string> permissions,
        CancellationToken ct);
}
