using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Courses;
using Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Courses.IntegrationEvents;

public sealed class CourseUpsertedIntegrationEventHandler
    : IIntegrationEventHandler<CourseUpsertedIntegrationEvent>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ILogger<CourseUpsertedIntegrationEventHandler> _logger;

    public CourseUpsertedIntegrationEventHandler(
        IWriteDbContext dbContext,
        ILogger<CourseUpsertedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(
        CourseUpsertedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var courseId = new ExternalCourseId(integrationEvent.CourseId);

        var existingCourse = await _dbContext.KnownCourses
            .FirstOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);

        if (existingCourse is null)
        {
            var newCourse = KnownCourse.Create(courseId, integrationEvent.Title, integrationEvent.IsActive);
            _dbContext.KnownCourses.Add(newCourse);
            _logger.LogInformation("Created KnownCourse for CourseId: {CourseId}", integrationEvent.CourseId);
        }
        else
        {
            existingCourse.Update(integrationEvent.Title, integrationEvent.IsActive);
            _logger.LogInformation("Updated KnownCourse for CourseId: {CourseId}", integrationEvent.CourseId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
