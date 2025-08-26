using AuthService.Models;

namespace AuthService.Settings;

public class UserDefaultsOptions
{
    public const string SectionName = "UserDefaults";

    public string DefaultRoleName { get; set; } = "User";
}