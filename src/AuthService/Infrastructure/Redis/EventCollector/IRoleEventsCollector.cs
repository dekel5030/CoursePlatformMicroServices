namespace Infrastructure.Redis.EventCollector;

public interface IRoleEventsCollector
{
    void MarkRoleForRefresh(string roleName);
    Task FlushAsync(CancellationToken ct);
}