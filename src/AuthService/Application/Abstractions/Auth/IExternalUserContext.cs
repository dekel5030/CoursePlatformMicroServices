namespace Application.Abstractions.Auth;

public interface IExternalUserContext
{
    string IdentityId { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    DateTime ExpiryUtc { get; }
}