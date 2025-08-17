using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Models;
using MassTransit;

namespace EnrollmentService.Messaging.Consumers;

public class CourseRemovedConsumer : IConsumer<EventEnvelope<CourseRemovedV1>>
{
    private readonly EnrollmentDbContext _dbContext;

    public CourseRemovedConsumer(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<CourseRemovedV1>> context)
    {
        EventEnvelope<CourseRemovedV1> envelope = context.Message;
        CourseRemovedV1 message = envelope.Payload;

        KnownCourse? course = await _dbContext.KnownCourses.FindAsync(message.CourseId);

        if (course is null)
        {
            course = new KnownCourse
            {
                CourseId = message.CourseId,
                IsAvailable = false,
                UpdatedAtUtc = envelope.OccurredAtUtc
            };

            _dbContext.KnownCourses.Add(course);
        }
        else if (envelope.OccurredAtUtc > course.UpdatedAtUtc)
        {
            course.IsAvailable = false;
            course.UpdatedAtUtc = envelope.OccurredAtUtc;
        }

        await _dbContext.SaveChangesAsync();
    }
}
