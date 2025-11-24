using Application.Abstractions.Messaging;
using Domain.Enrollments.Events;
using Enrollments.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Application.Enrollments.DomainEvents;

public sealed class EnrollmentCreatedDomainEventHandler
    : IDomainEventHandler<EnrollmentCreatedDomainEvent>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<EnrollmentCreatedDomainEventHandler> _logger;

    public EnrollmentCreatedDomainEventHandler(
        IEventPublisher eventPublisher,
        ILogger<EnrollmentCreatedDomainEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(
        EnrollmentCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new EnrollmentUpsertedV1(
            EnrollmentId: 0, // Will be mapped properly in infrastructure
            UserId: domainEvent.UserId.Value,
            CourseId: domainEvent.CourseId.Value,
            IsActive: true);

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation("Published EnrollmentUpserted for EnrollmentId: {EnrollmentId}",
            domainEvent.EnrollmentId.Value);
    }
}
