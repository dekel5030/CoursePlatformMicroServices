using Courses.Application.Shared.Dtos;

namespace Courses.Application.Modules.Dtos;

public sealed record ModuleCollectionDto : PaginatedCollectionDto<ModuleDetailsDto>;
