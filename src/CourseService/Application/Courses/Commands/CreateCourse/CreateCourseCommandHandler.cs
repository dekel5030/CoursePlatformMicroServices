using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

internal sealed class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<CreateCourseResponse>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        UserId instructorId = new(_userContext.Id ?? Guid.Empty);

        if (instructorId.Value == Guid.Empty)
        {
            return Result.Failure<CreateCourseResponse>(CourseErrors.Unauthorized);
        }

        Result<Course> courseResult = Course.CreateCourse(
            instructorId,
            request.Title,
            request.Description);

        if (courseResult.IsFailure)
        {
            return Result.Failure<CreateCourseResponse>(courseResult.Error);
        }

        Course course = courseResult.Value;

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCourseResponse(course.Id, course.Title));
    }
}
