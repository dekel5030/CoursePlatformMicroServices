using Application.Abstractions.Messaging;
using Auth.Contracts.Redis.Events;

namespace Infrastructure.Redis.EventCollector;

internal class RoleEventsCollector : IRoleEventsCollector
{
    private readonly IEventPublisher _publishEndpoint;
    private readonly HashSet<string> _rolesToRefresh = new();

    public RoleEventsCollector(IEventPublisher publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public void MarkRoleForRefresh(string roleName)
    {
        _rolesToRefresh.Add(roleName);
    }

    public async Task FlushAsync(CancellationToken ct)
    {
        foreach (var roleName in _rolesToRefresh)
        {
            await _publishEndpoint.PublishAsync(new RolePermissionsChangedEvent(roleName), ct);
        }

        _rolesToRefresh.Clear();
    }
}