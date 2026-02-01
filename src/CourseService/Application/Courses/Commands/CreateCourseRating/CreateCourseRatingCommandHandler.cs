using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Errors;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourseRating;

internal sealed class CreateCourseRatingCommandHandler : ICommandHandler<CreateCourseRatingCommand, CreateCourseRatingResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseRatingRepository _courseRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateCourseRatingCommandHandler(
        ICourseRepository courseRepository,
        ICourseRatingRepository courseRatingRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _courseRepository = courseRepository;
        _courseRatingRepository = courseRatingRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<CreateCourseRatingResponse>> Handle(
        CreateCourseRatingCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(_userContext.Id ?? Guid.Empty);

        if (userId.Value == Guid.Empty)
        {
            return Result.Failure<CreateCourseRatingResponse>(RatingErrors.Unauthorized);
        }

        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CreateCourseRatingResponse>(CourseErrors.NotFound);
        }

        IReadOnlyList<CourseRating> existingRatings = await _courseRatingRepository.ListAsync(
            rating => rating.CourseId == request.CourseId && rating.UserId == userId,
            cancellationToken);

        if (existingRatings.Count > 0)
        {
            return Result.Failure<CreateCourseRatingResponse>(RatingErrors.AlreadyRated);
        }

        Result<CourseRating> createResult = CourseRating.CreateRate(
            request.CourseId,
            userId,
            request.Score,
            request.Comment);

        if (createResult.IsFailure)
        {
            return Result.Failure<CreateCourseRatingResponse>(createResult.Error);
        }

        await _courseRatingRepository.AddAsync(createResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        CourseRating rating = createResult.Value;

        return Result.Success(new CreateCourseRatingResponse(
            rating.Id.Value,
            rating.CourseId.Value,
            rating.Score));
    }
}
