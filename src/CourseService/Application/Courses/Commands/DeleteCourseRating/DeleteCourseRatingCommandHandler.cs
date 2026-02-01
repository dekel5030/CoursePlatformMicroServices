using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Errors;
using Courses.Domain.Ratings.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.DeleteCourseRating;

internal sealed class DeleteCourseRatingCommandHandler : ICommandHandler<DeleteCourseRatingCommand>
{
    private readonly ICourseRatingRepository _courseRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public DeleteCourseRatingCommandHandler(
        ICourseRatingRepository courseRatingRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _courseRatingRepository = courseRatingRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(
        DeleteCourseRatingCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(_userContext.Id ?? Guid.Empty);

        if (userId.Value == Guid.Empty)
        {
            return Result.Failure(RatingErrors.Unauthorized);
        }

        CourseRating? rating = await _courseRatingRepository
            .GetByIdAsync(request.RatingId, cancellationToken);

        if (rating is null)
        {
            return Result.Failure(RatingErrors.NotFound);
        }

        if (rating.UserId != userId)
        {
            return Result.Failure(RatingErrors.Unauthorized);
        }

        _courseRatingRepository.Remove(rating!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
