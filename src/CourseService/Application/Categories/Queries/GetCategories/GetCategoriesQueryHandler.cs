using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
using Courses.Domain.Categories;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, CategoryCollectionDto>
{
    private readonly IReadDbContext _readDbContext;

    public GetCategoriesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<CategoryCollectionDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken = default)
    {
        int categoryCount = await _readDbContext.Categories.CountAsync(cancellationToken);

        List<Category> categories = await _readDbContext.Categories.ToListAsync(cancellationToken);

        var categoryDtos = categories
            .Select(c => new CategoryDto(c.Id.Value, c.Name, c.Slug.Value))
            .ToList();

        var response = new CategoryCollectionDto(categoryDtos, 1, categoryCount, categoryCount);

        return Result<CategoryCollectionDto>.Success(response);
    }
}
