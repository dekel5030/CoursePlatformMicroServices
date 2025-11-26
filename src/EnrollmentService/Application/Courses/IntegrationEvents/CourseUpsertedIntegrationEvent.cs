using Application.Abstractions.Messaging;

namespace Application.Courses.IntegrationEvents;

public sealed record CourseUpsertedIntegrationEvent(
    string CourseId,
    string Title,
    bool IsActive) : IIntegrationEvent;
