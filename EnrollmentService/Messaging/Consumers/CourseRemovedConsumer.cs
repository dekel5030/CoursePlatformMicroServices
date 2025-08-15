using Courses.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Models;
using MassTransit;

namespace EnrollmentService.Messaging.Consumers;

public class CourseRemovedConsumer : IConsumer<CourseRemovedV1>
{
    private readonly EnrollmentDbContext _dbContext;

    public CourseRemovedConsumer(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CourseRemovedV1> context)
    {
        CourseRemovedV1 message = context.Message;

        KnownCourse? course = await _dbContext.KnownCourses.FindAsync(message.CourseId);

        if (course is null)
        {
            course = new KnownCourse
            {
                CourseId = message.CourseId,
                Version = message.Version,
                IsAvailable = false,
                UpdatedAtUtc = message.UpdatedAtUtc
            };

            _dbContext.KnownCourses.Add(course);
        }
        else if (message.Version > course.Version)
        {
            course.Version = message.Version;
            course.IsAvailable = false;
            course.UpdatedAtUtc = message.UpdatedAtUtc;
        }

        await _dbContext.SaveChangesAsync();
    }
}
