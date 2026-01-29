using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

internal sealed class CategoryProjector :
    IEventConsumer<CategoryCreatedIntegrationEvent>,
    IEventConsumer<CategoryRenamedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CategoryProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        CategoryCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task HandleAsync(
        CategoryRenamedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        List<CourseHeaderReadModel> courseHeaders = await _readDbContext.CourseHeaders
            .Where(h => h.CategoryId == message.Id)
            .ToListAsync(cancellationToken);

        foreach (CourseHeaderReadModel header in courseHeaders)
        {
            header.CategoryName = message.NewName;
            header.CategorySlug = message.NewSlug;
            header.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        if (courseHeaders.Count > 0)
        {
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
