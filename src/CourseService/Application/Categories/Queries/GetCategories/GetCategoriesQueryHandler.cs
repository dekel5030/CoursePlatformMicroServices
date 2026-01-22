using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Categories.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public GetCategoriesQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        const string sql = @"
            SELECT 
                id,
                name,
                slug
            FROM categories
            ORDER BY name";

        IEnumerable<CategoryRow> categories = await connection.QueryAsync<CategoryRow>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        List<CategoryDto> categoryDtos = categories
            .Select(c => new CategoryDto(
                new CategoryId(c.Id),
                c.Name,
                new Slug(c.Slug)))
            .ToList();

        return Result.Success<IReadOnlyList<CategoryDto>>(categoryDtos);
    }
}
