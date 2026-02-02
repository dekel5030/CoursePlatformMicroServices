using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Categories.Queries.GetByIds;

internal sealed class GetCategoriesByIdsQueryHandler
    : IQueryHandler<GetCategoriesByIdsQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetCategoriesByIdsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesByIdsQuery request,
        CancellationToken cancellationToken = default)
    {
        var ids = request.Ids
            .Distinct()
            .Select(id => new CategoryId(id))
            .ToList();

        if (ids.Count == 0)
        {
            return Result.Success<IReadOnlyList<CategoryDto>>([]);
        }

        List<CategoryDto> categoryDtos = await _readDbContext.Categories
            .Where(category => ids.Contains(category.Id)) 
            .Select(category => new CategoryDto(category.Id.Value, category.Name, category.Slug.Value))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CategoryDto>>(categoryDtos);
    }
}
