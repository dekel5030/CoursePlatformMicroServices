using Application.Abstractions.Caching;
using Auth.Contracts.Dtos;
using Kernel.Auth;

namespace Application.AuthUsers.Queries.GetUserAuthData;

public sealed record GetUserAuthDataQuery(Guid UserId) : ICacheableQuery<UserAuthDataDto>
{
    public string CacheKey => string.Format(AuthCacheKeys.UserAuthDataPattern, UserId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
