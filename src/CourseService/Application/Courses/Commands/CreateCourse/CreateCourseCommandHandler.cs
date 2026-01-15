using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

internal sealed class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseDto>
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

    public async Task<Result<CreateCourseDto>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        InstructorId? instructorId = request.InstructorId.HasValue ? new(request.InstructorId.Value) : null;

        Result<Course> courseResult = Course.CreateCourse(
            _timeProvider,
            request.Title,
            request.Description,
            instructorId);

        if (courseResult.IsFailure)
        {
            return Result.Failure<CreateCourseDto>(courseResult.Error);
        }

        Course course = courseResult.Value;

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new CreateCourseDto(course.Id, course.Title.Value);

        return Result.Success(responseDto);
    }
}
