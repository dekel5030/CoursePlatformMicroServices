using System.Text.Json;
using Auth.Contracts.Redis;
using StackExchange.Redis;

namespace Infrastructure.Redis;

internal sealed class RedisRolePermissionsCacheWriter
    : IRolePermissionsCacheWriter
{
    private readonly IDatabase _db;

    public RedisRolePermissionsCacheWriter(
        IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task UpdateAsync(
        string roleName,
        IReadOnlyCollection<string> permissions,
        CancellationToken ct)
    {
        var model = new RolePermissionsCacheModel(
            roleName,
            permissions);

        var json = JsonSerializer.Serialize(model);

        await _db.StringSetAsync(
            RedisKeys.RolePermissions(roleName),
            json);
    }
}
