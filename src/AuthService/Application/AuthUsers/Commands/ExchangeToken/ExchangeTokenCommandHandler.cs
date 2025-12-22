using Auth.Application.Abstractions.Auth;
using Auth.Application.Abstractions.Caching;
using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Primitives;
using Auth.Domain.Roles;
using Kernel;
using Kernel.Auth;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Commands.ExchangeToken;

internal class ExchangeTokenCommandHandler : ICommandHandler<ExchangeTokenCommand, TokenResponse>
{
    private readonly IExternalUserContext _externalUserContext;
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPermissionResolver _permissionResolver;
    private readonly ICacheService _cacheService;

    public ExchangeTokenCommandHandler(
        IExternalUserContext externalUserContext,
        IWriteDbContext dbContext,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider,
        IPermissionResolver permissionResolver,
        ICacheService cacheService)
    {
        _externalUserContext = externalUserContext;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _tokenProvider = tokenProvider;
        _permissionResolver = permissionResolver;
        _cacheService = cacheService;
    }

    public async Task<Result<TokenResponse>> Handle(
        ExchangeTokenCommand request, 
        CancellationToken cancellationToken = default)
    {
        IdentityProviderId externalId = new(_externalUserContext.IdentityId);

        AuthUser? user = await _dbContext.Users.Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.IdentityId == externalId, cancellationToken);

        if (user == null)
        {
            user = await ProvisionUserAsync(cancellationToken);
        }

        var effectivePermissions = _permissionResolver.ResolveEffectivePermissions(user);

        var token = _tokenProvider.GenerateToken(user, effectivePermissions, _externalUserContext.ExpiryUtc);

        await _cacheService.SetAsync(
            AuthCacheKeys.UserInternalJwt(externalId.ProviderId),
            token,
            _externalUserContext.ExpiryUtc - DateTime.UtcNow,
            cancellationToken);

        return Result.Success(new TokenResponse(token));
    }

    private async Task<AuthUser> ProvisionUserAsync(CancellationToken cancellationToken)
    {
        IdentityProviderId externalId = new(_externalUserContext.IdentityId);
        FirstName firstName = new(_externalUserContext.FirstName);
        LastName lastName = new(_externalUserContext.LastName);
        FullName fullName = new(firstName, lastName);
        Email email = new(_externalUserContext.Email);

        Role? defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(role => role.Name == "user", cancellationToken);
        Result<AuthUser> userCreationResult = AuthUser.Create(externalId, fullName, email, defaultRole!);
        AuthUser newUser = userCreationResult.Value!;

        await _dbContext.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return newUser;
    }
}