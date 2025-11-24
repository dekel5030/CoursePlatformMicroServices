using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IWriteDbContext _dbContext;

    public LogoutCommandHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken = default)
    {
        var authUser = await _dbContext.AuthUsers
            .FirstOrDefaultAsync(user => user.RefreshToken == request.RefreshToken, cancellationToken);

        if (authUser == null)
        {
            return Result.Success();
        }

        authUser.ClearRefreshToken();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
