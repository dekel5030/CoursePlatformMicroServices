using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
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
        IQueryable<Domain.Categories.Category> query = _readDbContext.Categories.AsNoTracking();

        if (request.Filter.CourseId is { } courseId)
        {
            CategoryId? categoryId = await _readDbContext.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync(cancellationToken);

            if (categoryId is null)
            {
                return Result.Success<IReadOnlyList<CategoryDto>>([]);
            }

            query = query.Where(c => c.Id == categoryId);
        }

        if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new CategoryId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }
        }

        List<CategoryDto> categoryDtos = await query
            .Select(c => new CategoryDto(c.Id.Value, c.Name, c.Slug.Value))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CategoryDto>>(categoryDtos);
    }
}
