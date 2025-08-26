using Common.Messaging.EventEnvelope;

namespace CourseService.Services.Handlers;

public interface IEnvelopeHandler<T>
{
    Task HandleAsync(EventEnvelope<T> envelope, CancellationToken ct = default);
}