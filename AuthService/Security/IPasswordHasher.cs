namespace AuthService.Security;

public interface IPasswordHasher
{
    string Hash(string password);
}