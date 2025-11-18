# Security Improvements - RSA Signing and Hashed Refresh Tokens

## Overview
This update improves the security of the AuthService by implementing:
1. **RSA Asymmetric Signing** for JWT access tokens (replacing HMAC symmetric signing)
2. **SHA-256 Hashing** for refresh tokens before database storage

## Changes Made

### 1. RSA Asymmetric Signing for JWT

**Why?**
- **Better Security**: Private key stays on the auth service, public key can be distributed to all services
- **Key Rotation**: Easier to rotate keys without updating all services
- **Industry Standard**: RSA-256 is a widely accepted standard for JWT signing

**Implementation:**
- `TokenService.GenerateToken()` now uses RSA private key to sign JWTs
- `AuthenticationExtensions.AddJwtAuthentication()` uses RSA public key to validate JWTs
- JWT payload remains readable (not encrypted), ensuring integrity and authenticity through signature

**Configuration:**
```json
{
  "Jwt": {
    "PrivateKey": "-----BEGIN RSA PRIVATE KEY-----\n...\n-----END RSA PRIVATE KEY-----",
    "PublicKey": "-----BEGIN PUBLIC KEY-----\n...\n-----END PUBLIC KEY-----",
    "Issuer": "AuthService",
    "Audience": "CoursePlatform",
    "ExpirationMinutes": "60"
  }
}
```

### 2. SHA-256 Hashed Refresh Tokens

**Why?**
- **Rainbow Table Protection**: Even if database is compromised, attackers cannot use stored hashes
- **Best Practice**: Never store sensitive tokens in plaintext
- **Defense in Depth**: Additional security layer

**Implementation:**
- `ITokenService.HashRefreshToken()` - New method using SHA-256 to hash tokens
- `RegisterUserCommandHandler` - Hashes refresh token before storing
- `LoginUserCommandHandler` - Hashes refresh token before storing
- `RefreshTokenCommandHandler` - Hashes incoming token and compares with stored hash
- `AuthUser.SetRefreshToken()` - Now expects and stores the hash (parameter renamed for clarity)

**Flow:**
1. Generate random 64-byte refresh token
2. Hash it with SHA-256
3. Store hash in database
4. Send plain token to client in HttpOnly cookie
5. On refresh: Hash incoming token and compare with stored hash

## Generating RSA Keys

### Using the Provided Script (Linux/macOS):
```bash
cd src/AuthService
./generate-jwt-keys.sh
```

### Manual Generation:
```bash
# Generate private key
openssl genrsa -out jwt_private.pem 2048

# Extract public key
openssl rsa -in jwt_private.pem -pubout -out jwt_public.pem
```

### For Production:
- **DO NOT** store keys in appsettings.json
- Use environment variables:
  ```bash
  export JWT__PRIVATEKEY="$(cat jwt_private.pem)"
  export JWT__PUBLICKEY="$(cat jwt_public.pem)"
  ```
- Or use a secure key vault (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)

## Migration Notes

### Breaking Changes
- Configuration schema changed: `Jwt:Key` replaced with `Jwt:PrivateKey` and `Jwt:PublicKey`
- Existing refresh tokens in database are now invalid (they're plaintext, not hashes)

### Migration Steps

1. **Generate RSA Key Pair**
   ```bash
   cd src/AuthService
   ./generate-jwt-keys.sh
   ```

2. **Update Configuration**
   - Replace `Jwt:Key` with `Jwt:PrivateKey` and `Jwt:PublicKey` in appsettings.json
   - For production, use environment variables or key vault

3. **Clear Existing Refresh Tokens** (Optional but recommended)
   ```sql
   UPDATE auth_users SET refresh_token = NULL, refresh_token_expires_at = NULL;
   ```
   This forces users to re-authenticate, getting new hashed tokens.

4. **Deploy Updated Service**
   - AuthService will start signing tokens with RSA
   - Old HMAC-signed tokens will be rejected (users must re-login)

5. **Update Other Services** (If they validate tokens)
   - Provide them with the RSA public key
   - Update their validation to use `RsaSecurityKey` instead of `SymmetricSecurityKey`

## Security Benefits

### RSA Signing
✅ **Public Key Distribution**: Other services can validate tokens without accessing signing key  
✅ **Key Compromise Mitigation**: If a service is compromised, they can't sign new tokens  
✅ **Easier Key Rotation**: Distribute new public key to services, rotate private key on auth service  
✅ **Audit Trail**: Signature proves token was issued by legitimate auth service  

### Hashed Refresh Tokens
✅ **Database Breach Protection**: Stolen hashes cannot be used as refresh tokens  
✅ **No Rainbow Tables**: SHA-256 with random input makes precomputation infeasible  
✅ **Compliance**: Meets security standards for storing authentication credentials  
✅ **Defense in Depth**: Additional layer even with encrypted database  

## Code Changes Summary

### Modified Files
- `Application/Abstractions/Security/ITokenService.cs` - Added `HashRefreshToken()` method
- `Infrastructure/Security/TokenService.cs` - Implemented RSA signing and SHA-256 hashing
- `Web.Api/Extensions/AuthenticationExtensions.cs` - Updated to use RSA public key
- `Domain/AuthUsers/AuthUser.cs` - Updated method parameters to reflect hash storage
- `Application/AuthUsers/Commands/RegisterUser/RegisterUserCommandHandler.cs` - Hash before storing
- `Application/AuthUsers/Commands/LoginUser/LoginUserCommandHandler.cs` - Hash before storing
- `Application/AuthUsers/Commands/RefreshToken/RefreshTokenCommandHandler.cs` - Hash before comparing
- `Web.Api/appsettings.json` - Updated JWT configuration schema

### New Files
- `src/AuthService/generate-jwt-keys.sh` - Script to generate RSA key pairs
- `src/AuthService/SECURITY_IMPROVEMENTS.md` - This documentation file

## Testing

### Verify RSA Signing
1. Register a new user
2. Decode the JWT from the cookie using https://jwt.io
3. Verify the algorithm is `RS256` (not `HS256`)
4. Verify the signature validates with the public key

### Verify Hashed Refresh Tokens
1. Login and check database: `SELECT refresh_token FROM auth_users WHERE email = 'test@example.com';`
2. Verify the stored value is a Base64-encoded hash (44 characters for SHA-256)
3. Try using the database value as refresh token - it should fail
4. Use the actual cookie value to refresh - it should succeed

## Backward Compatibility

⚠️ **Not Backward Compatible**
- Existing JWT tokens signed with HMAC will be rejected
- Existing plaintext refresh tokens in database are invalid
- All users must re-authenticate after deployment

**Recommendation**: Schedule deployment during low-traffic period and communicate the need to re-login to users.

## Performance Impact

- **Token Generation**: Slightly slower (RSA vs HMAC), but negligible for auth operations
- **Token Validation**: Similar performance (RSA public key verification is fast)
- **Hashing**: SHA-256 is very fast, no noticeable impact
- **Database**: Hash comparison same as string comparison

## Security Audit Checklist

- [x] Private key never leaves auth service
- [x] Public key can be safely distributed
- [x] Refresh tokens never stored in plaintext
- [x] SHA-256 used for hashing (not MD5 or SHA-1)
- [x] Tokens still transmitted in HttpOnly, Secure, SameSite cookies
- [x] Token rotation still implemented
- [x] Expiration times unchanged (60 min access, 7 day refresh)

## References

- [RFC 7519 - JSON Web Token (JWT)](https://datatracker.ietf.org/doc/html/rfc7519)
- [RFC 7518 - JSON Web Algorithms (JWA)](https://datatracker.ietf.org/doc/html/rfc7518)
- [OWASP - Cryptographic Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)
- [NIST - Secure Hash Standard (SHA-256)](https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.180-4.pdf)
