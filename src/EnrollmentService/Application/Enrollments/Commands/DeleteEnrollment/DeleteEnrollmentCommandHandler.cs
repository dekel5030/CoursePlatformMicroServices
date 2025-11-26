using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Enrollments.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Enrollments.Commands.DeleteEnrollment;

public sealed class DeleteEnrollmentCommandHandler
    : ICommandHandler<DeleteEnrollmentCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ILogger<DeleteEnrollmentCommandHandler> _logger;

    public DeleteEnrollmentCommandHandler(
        IWriteDbContext dbContext,
        ILogger<DeleteEnrollmentCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeleteEnrollmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (enrollment is null)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} not found", command.Id);
            return Result.Failure(EnrollmentErrors.EnrollmentNotFound);
        }

        _dbContext.Enrollments.Remove(enrollment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enrollment {EnrollmentId} deleted", command.Id);

        return Result.Success();
    }
}
