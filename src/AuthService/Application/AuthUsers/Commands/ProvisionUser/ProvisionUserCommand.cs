using Application.Abstractions.Caching;
using Application.Abstractions.Messaging;
using Auth.Contracts.Dtos;
using Kernel.Auth;

namespace Application.AuthUsers.Commands.ProvisionUser;

public sealed record ProvisionUserCommand(string IdentityUserId)
    : ICommand<UserAuthDataDto>, ICacheableQuery<UserAuthDataDto>
{
    public string CacheKey => string.Format(AuthCacheKeys.UserAuthDataPattern, IdentityUserId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
