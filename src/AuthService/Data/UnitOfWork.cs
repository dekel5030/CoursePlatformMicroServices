using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;

namespace AuthService.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _dbContext;

    public IAuthUserRepository AuthUserRepository { get; }
    public IPermissionRepository PermissionRepository { get; }
    public IRoleRepository RoleRepository { get; }

    public UnitOfWork(AuthDbContext dbContext, IAuthUserRepository authUserRepository, IPermissionRepository permissionRepository, IRoleRepository roleRepository)
    {
        _dbContext = dbContext;
        AuthUserRepository = authUserRepository;
        PermissionRepository = permissionRepository;
        RoleRepository = roleRepository;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}