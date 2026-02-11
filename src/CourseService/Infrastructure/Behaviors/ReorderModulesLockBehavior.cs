using Courses.Application.Modules.Commands.ReorderModules;
using Courses.Infrastructure.Database.Write;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Courses.Infrastructure.Behaviors;

internal sealed class ReorderModulesLockBehavior<TRequest, TResponse>
    : IPipelineBehavior<ReorderModulesCommand, TResponse>
    where TResponse : Result
{
    private readonly WriteDbContext _context;

    public ReorderModulesLockBehavior(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(
        ReorderModulesCommand request,
        RequestHandlerDelegate<TResponse> nextHandler,
        CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Modules
                .Where(module => module.CourseId == request.CourseId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(module => module.Index, l => l.Index * -1 - 100),
                    cancellationToken);


            TResponse response = await nextHandler();

            if (response.IsSuccess)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
