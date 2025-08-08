using AuthService.Models;

public interface IUserDefaultsService
{
    Task<Role> GetDefaultRoleAsync();
}
