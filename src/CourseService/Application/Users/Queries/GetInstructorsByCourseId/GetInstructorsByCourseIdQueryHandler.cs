using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Users.Queries.GetByIds;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Users.Queries.GetInstructorsByCourseId;

public sealed class GetInstructorsByCourseIdQueryHandler
    : IQueryHandler<GetInstructorsByCourseIdQuery, IReadOnlyList<UserDto>>
{
    private readonly IMediator _mediator;

    public GetInstructorsByCourseIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetInstructorsByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Result<CourseDto> course = await _mediator.Send(
            new GetCourseByIdQuery(request.CourseId),
            cancellationToken);

        if (course.IsFailure)
        {
            return Result.Failure<IReadOnlyList<UserDto>>(course.Error);
        }

        CourseDto courseDto = course.Value;

        Result<IReadOnlyList<UserDto>> instructorsDtos = await _mediator.Send(
            new GetInstructorsByIdsQuery(new List<Guid> { courseDto.InstructorId }), cancellationToken);

        if (instructorsDtos.IsFailure)
        {
            return Result.Failure<IReadOnlyList<UserDto>>(instructorsDtos.Error);
        }

        return Result.Success(instructorsDtos.Value);
    }
}
