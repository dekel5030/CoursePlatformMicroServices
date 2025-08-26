using Common.Messaging.EventEnvelope;

namespace EnrollmentService.Services.EnrollmentMessageHandler;

public interface IEnvelopeHandler<T>
{
    Task HandleAsync(EventEnvelope<T> envelope, CancellationToken ct = default);
}