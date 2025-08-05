namespace AuthService.Validators;

public class ValidationSettings
{
    public int PasswordMinLength { get; set; }
    public int PasswordMaxLength { get; set; }
    public bool PasswordRequireUppercase { get; set; }
    public bool PasswordRequireLowercase { get; set; }
    public bool PasswordRequireDigit { get; set; }
    public bool PasswordRequireSpecial { get; set; }
    
    public int EmailMaxLength { get; set; } 
    public int FullNameMaxLength { get; set; }
}
