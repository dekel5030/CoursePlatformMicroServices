namespace AuthService.Services;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionAsync(int userId);
}