using Courses.Application.Lessons.Commands.ReorderLessons;
using Courses.Infrastructure.Database.Write;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Courses.Infrastructure.Behaviors;

internal sealed class ReorderLessonsLockBehavior<TRequest, TResponse>
    : IPipelineBehavior<ReorderLessonsCommand, TResponse>
    where TResponse : Result
{
    private readonly WriteDbContext _context;

    public ReorderLessonsLockBehavior(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(
        ReorderLessonsCommand request,
        RequestHandlerDelegate<TResponse> nextHandler,
        CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Lessons
                .Where(l => l.ModuleId == request.ModuleId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(l => l.Index, l => l.Index * -1 - 100),
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
