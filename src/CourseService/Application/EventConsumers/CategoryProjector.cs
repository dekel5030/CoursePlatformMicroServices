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
        var categoryReadModel = new CategoryReadModel
        {
            Id = message.Id,
            Name = message.Name,
            Slug = message.Slug
        };

        _readDbContext.Categories.Add(categoryReadModel);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task HandleAsync(
        CategoryRenamedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        CategoryReadModel? category = await _readDbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == message.Id, cancellationToken);

        if (category is not null)
        {
            category.Name = message.NewName;
            category.Slug = message.NewSlug;
        }

        List<CourseHeaderReadModel> courseHeaders = await _readDbContext.CourseHeaders
            .Where(h => h.CategoryId == message.Id)
            .ToListAsync(cancellationToken);

        foreach (CourseHeaderReadModel header in courseHeaders)
        {
            header.CategoryName = message.NewName;
            header.CategorySlug = message.NewSlug;
            header.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        List<CourseListItemReadModel> courseListItems = await _readDbContext.CourseListItems
            .Where(c => c.CategoryId == message.Id)
            .ToListAsync(cancellationToken);

        foreach (CourseListItemReadModel item in courseListItems)
        {
            item.CategoryName = message.NewName;
            item.CategorySlug = message.NewSlug;
            item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        if (category is not null || courseHeaders.Count > 0 || courseListItems.Count > 0)
        {
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
