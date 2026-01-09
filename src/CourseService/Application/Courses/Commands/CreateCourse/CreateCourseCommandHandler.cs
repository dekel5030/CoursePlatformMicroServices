using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

internal class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository, 
        TimeProvider timeProvider, 
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCourseResponse>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        Title? title = string.IsNullOrEmpty(request.Title) ? null : new(request.Title);
        Description? description = string.IsNullOrEmpty(request.Description) ? null : new(request.Description);
        InstructorId? instructorId = request.InstructorId.HasValue ? new(request.InstructorId.Value) : null;

        Result<Course> courseResult = Course.CreateCourse(
            _timeProvider,
            title,
            description,
            instructorId);

        if (courseResult.IsFailure)
        {
            return Result.Failure<CreateCourseResponse>(courseResult.Error);
        }

        Course course = courseResult.Value;

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new CreateCourseResponse(course.Id.Value, course.Title.Value);

        return Result.Success(responseDto);
    }
}
