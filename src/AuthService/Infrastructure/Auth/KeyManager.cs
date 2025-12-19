using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class KeyManager
{
    private readonly RSA _rsa;

    public KeyManager()
    {
        _rsa = RSA.Create(2048);
    }

    public RsaSecurityKey GetPrivateKey() => new RsaSecurityKey(_rsa);

    public RsaSecurityKey GetPublicKey()
    {
        var publicRsa = RSA.Create();
        publicRsa.ImportParameters(_rsa.ExportParameters(false));
        return new RsaSecurityKey(publicRsa);
    }

    public string GetKeyId() => "internal-auth-key-v1";
}