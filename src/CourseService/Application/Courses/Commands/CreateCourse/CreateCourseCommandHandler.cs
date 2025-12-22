using Application.Abstractions.Data;
using Domain.Courses;
using Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CourseId>
{
    private readonly IWriteDbContext _dbContext;

    public CreateCourseCommandHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CourseId>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        CreateCourseDto dto = request.Dto;
        Money coursePrice = new(dto.PriceAmount ?? 0, dto.PriceCurrency ?? "ILS");
        var course = Course.CreateCourse(dto.Title, dto.Description, dto.ImageUrl, dto.InstructorId, coursePrice);

        await _dbContext.Courses.AddAsync(course, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(course.Id);
    }
}
