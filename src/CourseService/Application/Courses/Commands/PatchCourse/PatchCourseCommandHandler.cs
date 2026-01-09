using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

internal class PatchCourseCommandHandler : ICommandHandler<PatchCourseCommand>
{
    private readonly TimeProvider _timeProvider;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchCourseCommandHandler(TimeProvider timeProvider, ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _timeProvider = timeProvider;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

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

        if ((request.PriceAmount.HasValue) != (request.PriceCurrency is not null))
        {
            return Result.Failure(Error.Validation(
                "Course.IncompletePriceUpdate", 
                "Both price amount and currency must be provided together."));
        }
        
        if (request.PriceAmount.HasValue && request.PriceCurrency is not null)
        {
            var priceResult = course.SetPrice(new Money(request.PriceAmount.Value, request.PriceCurrency), _timeProvider);
            if (priceResult.IsFailure)
            {
                return priceResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
