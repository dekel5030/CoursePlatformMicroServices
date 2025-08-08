using AuthService.Data.Repositories.Implementations;
using AuthService.Models;
using AuthService.Settings;
using Microsoft.Extensions.Options;

public class UserDefaultsService : IUserDefaultsService
{
    private UserDefaultsOptions _options;
    private Role? _defaultRole;
    private readonly RoleRepository _roleRepository;

    public UserDefaultsService(IOptionsMonitor<UserDefaultsOptions> monitor, RoleRepository roleRepository)
    {
        _options = monitor.CurrentValue;
        _roleRepository = roleRepository;

        monitor.OnChange(updatedOptions => {
            _options = updatedOptions;
            _defaultRole = null;
        });
    }

    public async Task<Role> GetDefaultRoleAsync()
    {
        if (_defaultRole == null)
        {
            _defaultRole = await _roleRepository.GetRoleByNameAsync(_options.DefaultRoleName);
        }

        return _defaultRole ?? throw new InvalidOperationException("Default role not found.");
    }
}
