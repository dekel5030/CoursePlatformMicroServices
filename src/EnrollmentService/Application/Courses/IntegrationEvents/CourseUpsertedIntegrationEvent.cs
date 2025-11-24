using Application.Abstractions.Messaging;

namespace Application.Courses.IntegrationEvents;

public sealed record CourseUpsertedIntegrationEvent(
    int CourseId,
    string Title,
    bool IsActive) : IIntegrationEvent;
