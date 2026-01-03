using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Commands.PatchCourse;

internal class PatchCourseCommandHandler : ICommandHandler<PatchCourseCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public PatchCourseCommandHandler(IWriteDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(
        PatchCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.CourseId);

        var course = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        // Apply updates only for non-null fields
        if (request.Title is not null)
        {
            var titleResult = course.UpdateTitle(new Title(request.Title), _timeProvider);
            if (titleResult.IsFailure)
            {
                return titleResult;
            }
        }

        if (request.Description is not null)
        {
            var descriptionResult = course.UpdateDescription(new Description(request.Description), _timeProvider);
            if (descriptionResult.IsFailure)
            {
                return descriptionResult;
            }
        }

        if (request.InstructorId.HasValue)
        {
            var instructorResult = course.AssignInstructor(new InstructorId(request.InstructorId.Value), _timeProvider);
            if (instructorResult.IsFailure)
            {
                return instructorResult;
            }
        }

        // Validate price update: both amount and currency must be provided together
        if (request.PriceAmount.HasValue || request.PriceCurrency is not null)
        {
            if (!request.PriceAmount.HasValue || request.PriceCurrency is null)
            {
                return Result.Failure(Error.Validation(
                    "Course.IncompletePriceUpdate", 
                    "Both price amount and currency must be provided together."));
            }
            
            var priceResult = course.SetPrice(new Money(request.PriceAmount.Value, request.PriceCurrency), _timeProvider);
            if (priceResult.IsFailure)
            {
                return priceResult;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
