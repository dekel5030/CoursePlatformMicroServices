namespace Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
    
    // File paths for RSA keys (optional - if provided, keys will be loaded from files)
    public string? PrivateKeyFile { get; set; }
    public string? PublicKeyFile { get; set; }
    
    // Actual key content (loaded from files or directly from config)
    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}
