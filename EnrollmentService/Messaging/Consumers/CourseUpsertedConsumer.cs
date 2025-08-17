using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Models;
using MassTransit;

namespace EnrollmentService.Messaging.Consumers;

public class CourseUpsertedConsumer : IConsumer<EventEnvelope<CourseUpsertedV1>>
{
    private readonly EnrollmentDbContext _dbContext;

    public CourseUpsertedConsumer(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CourseUpsertedV1>> context)
    {
        EventEnvelope<CourseUpsertedV1> envelope = context.Message;
        CourseUpsertedV1 message = envelope.Payload;

        KnownCourse? course = await _dbContext.KnownCourses.FindAsync(message.CourseId);

        if (course is null)
        {
            course = new KnownCourse
            {
                CourseId = message.CourseId,
                IsAvailable = true,
                UpdatedAtUtc = envelope.OccurredAtUtc
            };

            _dbContext.KnownCourses.Add(course);
        }
        else if (envelope.OccurredAtUtc > course.UpdatedAtUtc)
        {
            course.IsAvailable = true;
            course.UpdatedAtUtc = envelope.OccurredAtUtc;
        }

        await _dbContext.SaveChangesAsync();
    }
}
