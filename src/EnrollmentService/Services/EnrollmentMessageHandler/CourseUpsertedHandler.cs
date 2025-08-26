using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Models;

namespace EnrollmentService.Services.EnrollmentMessageHandler;

public class CourseUpsertedHandler : IEnvelopeHandler<CourseUpsertedV1>
{
    private readonly EnrollmentDbContext _dbContext;
    private readonly ILogger<CourseUpsertedHandler> _logger;

    public CourseUpsertedHandler(EnrollmentDbContext dbContext, ILogger<CourseUpsertedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task HandleAsync(EventEnvelope<CourseUpsertedV1> envelope, CancellationToken ct = default)
    {
        CourseUpsertedV1 message = envelope.Payload;

        _logger.LogInformation("Received CourseUpserted event for CourseId={CourseId}, AggregateVersion={AggregateVersion}",
            message.CourseId, envelope.AggregateVersion);

        KnownCourse? course = await _dbContext.KnownCourses
            .FindAsync(new object?[] { message.CourseId }, ct);

        if (course is null)
        {
            _logger.LogInformation("KnownCourse not found for CourseId={CourseId}. Creating new KnownCourse.", message.CourseId);
            course = CreateCourse(envelope);
            await _dbContext.KnownCourses.AddAsync(course, ct);
        }
        else if (envelope.AggregateVersion > course.AggregateVersion)
        {
            _logger.LogInformation("Envelope version {EnvelopeVersion} is newer than stored {StoredVersion} for CourseId={CourseId}. Applying updates.",
                envelope.AggregateVersion, course.AggregateVersion, message.CourseId);
            ApplyCourseUpdates(course, envelope);
        }
        else
        {
            _logger.LogInformation("Skipping CourseUpserted for CourseId={CourseId}: envelope version {EnvelopeVersion} <= stored version {StoredVersion}.",
                message.CourseId, envelope.AggregateVersion, course.AggregateVersion);
            return;
        }

        _logger.LogInformation("Saving KnownCourse changes for CourseId={CourseId}.", message.CourseId);
        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Saved KnownCourse for CourseId={CourseId} at version {Version}.", message.CourseId, envelope.AggregateVersion);
    }

    private KnownCourse CreateCourse(EventEnvelope<CourseUpsertedV1> envelope)
    {
        _logger.LogInformation("Creating KnownCourse entity for CourseId={CourseId}.", envelope.Payload.CourseId);

        KnownCourse course = new KnownCourse
        {
            CourseId = envelope.Payload.CourseId,
            IsAvailable = envelope.Payload.IsPublished
        };

        ApplyCourseUpdates(course, envelope);

        _logger.LogInformation("Created KnownCourse entity for CourseId={CourseId}.", envelope.Payload.CourseId);

        return course;
    }

    private void ApplyCourseUpdates(KnownCourse course, EventEnvelope<CourseUpsertedV1> envelope)
    {
        _logger.LogInformation("Applying updates to KnownCourse CourseId={CourseId}: IsPublished={IsPublished}, OccurredAtUtc={OccurredAtUtc}, NewVersion={Version}.",
            envelope.Payload.CourseId, envelope.Payload.IsPublished, envelope.OccurredAtUtc, envelope.AggregateVersion);

        course.AggregateVersion = envelope.AggregateVersion;
        course.UpdatedAtUtc = envelope.OccurredAtUtc;
        course.IsAvailable = envelope.Payload.IsPublished;

        _logger.LogInformation("Applied updates to KnownCourse CourseId={CourseId}; StoredVersion={StoredVersion}.",
            envelope.Payload.CourseId, course.AggregateVersion);
    }
}