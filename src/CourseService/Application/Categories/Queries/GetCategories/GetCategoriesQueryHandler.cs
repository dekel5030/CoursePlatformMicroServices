using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetCategoriesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> query = _readDbContext.Categories;

        query = ApplyFilters(request, query);

        List<CategoryDto> categoryDtos = await query
            .Select(course => new CategoryDto(course.Id.Value, course.Name, course.Slug.Value))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CategoryDto>>(categoryDtos);
    }

    private static IQueryable<Category> ApplyFilters(GetCategoriesQuery request, IQueryable<Category> query)
    {
        if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new CategoryId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(course => ids.Contains(course.Id));
            }
        }

        return query;
    }
}
