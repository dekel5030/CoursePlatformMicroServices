using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Auth;

public class KeyManager : IDisposable
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
        RSAParameters publicParams = _rsa.ExportParameters(false);
        return new RsaSecurityKey(publicParams);
    }

    public RSAParameters GetPublicKeyParameters() => _rsa.ExportParameters(false);

    public void Dispose()
    {
        _rsa.Dispose();
    }
}
