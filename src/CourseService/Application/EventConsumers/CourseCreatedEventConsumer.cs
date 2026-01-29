using System;
using System.Collections.Generic;
using System.Text;
using CoursePlatform.Contracts.CourseEvents;
using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.ReadModels;
using Courses.Domain.Courses.Primitives;
using Kernel.EventBus;

namespace Courses.Application.EventConsumers;

internal sealed class CourseCreatedEventConsumer : IEventConsumer<CourseCreatedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CourseCreatedEventConsumer(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public Task HandleAsync(CourseCreatedIntegrationEvent message, CancellationToken cancellationToken = default)
    {
        var coursePage = new CoursePage
        {
            Id = message.CourseId,
            Title = message.Title,
            InstructorId = message.InstructorId,
            PriceAmount = message.PriceAmount,
            PriceCurrency = message.PriceCurrency,
            Status = Enum.Parse<CourseStatus>(message.Status),
            UpdatedAtUtc = message.CreatedAt.UtcDateTime,
            Description = string.Empty,
            InstructorName = string.Empty,
            EnrollmentCount = 0,
            LessonsCount = 0,
            Duration = TimeSpan.Zero,
            ImageUrls = new List<string>(),
            Tags = new List<string>(),
            CategoryId = Guid.Empty,
            CategoryName = string.Empty,
            CategorySlug = string.Empty,
            Modules = new List<ModuleReadModel>()
        };

        _readDbContext.CoursePages.Add(coursePage);
        return _readDbContext.SaveChangesAsync(cancellationToken);
    }
}
