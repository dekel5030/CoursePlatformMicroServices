using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandler : IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetAllPermissionsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken cancellationToken = default)
    {
        var permissions = await _readDbContext.Permissions
            .Select(p => new PermissionDto(p.Id.Value, p.Name))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<PermissionDto>>(permissions);
    }
}
