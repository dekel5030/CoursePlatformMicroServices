using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Auth;

public class KeyManager
{
    private readonly RSA _rsa;
    public static string KeyId => "internal-auth-key-v1";

    public KeyManager()
    {
        _rsa = RSA.Create(2048);
    }

    public RsaSecurityKey PrivateKey => new RsaSecurityKey(_rsa);

    public RsaSecurityKey GetPublicKey()
    {
        var publicRsa = RSA.Create();
        publicRsa.ImportParameters(_rsa.ExportParameters(false));
        return new RsaSecurityKey(publicRsa);
    }
}