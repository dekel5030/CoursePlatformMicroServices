using Common.Messaging.EventEnvelope;
using CourseService.Data.EnrollmentsRepo;
using CourseService.Data.UnitOfWork;
using CourseService.Models;
using Enrollments.Contracts.Events;

namespace CourseService.Services.Handlers;

public class EnrollmentUpsertedHandler : IEnvelopeHandler<EnrollmentUpsertedV1>
{
    private readonly IEnrollmentsRepo _enrollmentsRepo;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentUpsertedHandler(IEnrollmentsRepo enrollmentsRepo, IUnitOfWork unitOfWork)
    {
        _enrollmentsRepo = enrollmentsRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(EventEnvelope<EnrollmentUpsertedV1> envelope, CancellationToken ct = default)
    {
        EnrollmentUpsertedV1 message = envelope.Payload;
        Enrollment? enrollment = await _enrollmentsRepo.GetByIdAsync(message.EnrollmentId, ct);

        if (enrollment is null)
        {
            Enrollment newEnrollment = CreateEnrollment(envelope);
            await _enrollmentsRepo.AddAsync(newEnrollment);
        }
        else if (envelope.AggregateVersion > enrollment.AggregateVersion)
        {
            UpdateEnrollment(envelope, enrollment);
        }
        else
        {
            return;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private void UpdateEnrollment(EventEnvelope<EnrollmentUpsertedV1> envelope, Enrollment enrollment)
    {
        enrollment.IsActive = envelope.Payload.IsActive;
        enrollment.AggregateVersion = envelope.AggregateVersion;
        enrollment.UpdatedAtUtc = envelope.OccurredAtUtc;
    }

    private Enrollment CreateEnrollment(EventEnvelope<EnrollmentUpsertedV1> envelope)
    {
        Enrollment newEnrollment = new Enrollment
        {
            EnrollmentId = envelope.Payload.EnrollmentId,
            UserId = envelope.Payload.UserId,
            CourseId = envelope.Payload.CourseId,
        };

        UpdateEnrollment(envelope, newEnrollment);

        return newEnrollment;
    }
}
