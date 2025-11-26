using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Domain.Enrollments.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Enrollments.Queries.GetEnrollmentById;

public sealed class GetEnrollmentByIdQueryHandler
    : IQueryHandler<GetEnrollmentByIdQuery, EnrollmentReadDto>
{
    private readonly IReadDbContext _dbContext;

    public GetEnrollmentByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<EnrollmentReadDto>> Handle(
        GetEnrollmentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await _dbContext.Enrollments
            .Where(e => e.Id == query.Id)
            .Select(e => new EnrollmentReadDto
            {
                Id = e.Id.Value.ToString(),
                UserId = e.UserId.Value,
                CourseId = e.CourseId.Value,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt,
                ExpiresAt = e.ExpiresAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
        {
            return Result.Failure<EnrollmentReadDto>(EnrollmentErrors.EnrollmentNotFound);
        }

        return Result.Success(enrollment);
    }
}