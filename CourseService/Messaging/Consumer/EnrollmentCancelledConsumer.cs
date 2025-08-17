using Common.Messaging.EventEnvelope;
using CourseService.Data;
using CourseService.Models;
using Enrollments.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Messaging.Consumer;

public class EnrollmentCancelledConsumer : IConsumer<EventEnvelope<EnrollmentCancelledV1>>
{
    private readonly CourseDbContext _dbContext;
    
    public EnrollmentCancelledConsumer(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<EnrollmentCancelledV1>> context)
    {
        EventEnvelope<EnrollmentCancelledV1> message = context.Message;
        CancellationToken ct = context.CancellationToken;

        Enrollment? enrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentId == message.Payload.EnrollmentId, ct);

        if (enrollment is null)
        {
            enrollment = new Enrollment
            {
                EnrollmentId = message.Payload.EnrollmentId,
                CourseId = message.Payload.CourseId,
                UserId = message.Payload.UserId,
                UpdatedAt = message.OccurredAtUtc,
                IsActive = false
            };

            await _dbContext.Enrollments.AddAsync(enrollment, ct);
        }
        else if (enrollment.UpdatedAt < message.OccurredAtUtc)
        {
            enrollment.IsActive = false;
            enrollment.UpdatedAt = message.OccurredAtUtc;
        }
        else
        {
            return;
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}