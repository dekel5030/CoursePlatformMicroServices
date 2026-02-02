using Courses.Application.Categories.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetByIds;

public sealed record GetCategoriesByIdsQuery(
    IEnumerable<Guid> Ids): IQuery<IReadOnlyList<CategoryDto>>;
