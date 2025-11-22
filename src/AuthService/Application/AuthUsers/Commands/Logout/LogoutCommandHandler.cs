using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public LogoutCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken = default)
    {
        // Find the user by email
        var authUser = await _readDbContext.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (authUser == null)
        {
            // User not found - still return success for security reasons
            // (don't reveal whether a user exists)
            return Result.Success();
        }

        // Clear the refresh token
        authUser.ClearRefreshToken();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
