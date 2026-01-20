using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Actions.Abstract;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

internal sealed class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CourseSummaryDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ICourseActionProvider _actionProvider;
    private readonly IUserContext _userContext;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork,
        ICourseActionProvider courseActionProvider,
        IUserContext userContext)
    {
        _courseRepository = courseRepository;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
        _actionProvider = courseActionProvider;
        _userContext = userContext;
    }

    public async Task<Result<CourseSummaryDto>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        UserId instructorId = new(_userContext.Id ?? Guid.Empty);

        if (instructorId.Value == Guid.Empty)
        {
            return Result.Failure<CourseSummaryDto>(CourseErrors.Unauthorized);
        }

        Result<Course> courseResult = Course.CreateCourse(
            _timeProvider,
            instructorId,
            request.Title,
            request.Description);

        if (courseResult.IsFailure)
        {
            return Result.Failure<CourseSummaryDto>(courseResult.Error);
        }

        Course course = courseResult.Value;

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Instructor?.FullName,
            course.Price.Amount,
            course.Price.Currency,
            null,
            course.LessonCount,
            course.EnrollmentCount,
            _actionProvider.GetAllowedActions(course)
        );

        return Result.Success(responseDto);
    }
}
