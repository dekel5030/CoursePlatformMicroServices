using System;
using System.Collections.Generic;
using System.Text;
using CoursePlatform.Contracts.UserEvents;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;
using Users.Domain.Users;
using Users.Domain.Users.Events;

namespace Users.Application.Users.Events.DomainEvents;

internal class UserProfileCreatedDomainEventHandler : IDomainEventHandler<UserProfileCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public UserProfileCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(
        UserProfileCreatedDomainEvent message, 
        CancellationToken cancellationToken = default)
    {
        User user = message.User;
        var integrationEvent = new UserCreated(
            user.Id.ToString(), 
            user.FullName?.FirstName ?? string.Empty,
            user.FullName?.LastName ?? string.Empty,
            user.Email,
            user.AvatarUrl);

        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
