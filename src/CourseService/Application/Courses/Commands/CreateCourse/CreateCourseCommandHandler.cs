//using Courses.Application.Abstractions.Data;
//using Courses.Domain.Courses;
//using Courses.Domain.Courses.Primitives;
//using Courses.Domain.Shared.Primitives;
//using Kernel;
//using Kernel.Messaging.Abstractions;

//namespace Courses.Application.Courses.Commands.CreateCourse;

//internal class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseResponse>
//{
//    private readonly IWriteDbContext _dbContext;
//    private readonly TimeProvider _timeProvider;

//    public CreateCourseCommandHandler(IWriteDbContext dbContext, TimeProvider timeProvider)
//    {
//        _dbContext = dbContext;
//        _timeProvider = timeProvider;
//    }

//    public async Task<Result<CreateCourseResponse>> Handle(
//        CreateCourseCommand request,
//        CancellationToken cancellationToken = default)
//    {
//        Title? title = !string.IsNullOrWhiteSpace(request.Title) ? new Title(request.Title) : null;
//        Description? description = !string.IsNullOrWhiteSpace(request.Description) ? new Description(request.Description) : null;
//        InstructorId? instructorId = request.InstructorId.HasValue ? new InstructorId(request.InstructorId.Value) : null;
//        Money? price = request.PriceAmount.HasValue ? new Money(request.PriceAmount.Value, request.PriceCurrency ?? "ILS") : null;

//        Result<Course> courseResult = Course.CreateCourse(
//            title,
//            description,
//            instructorId,
//            price,
//            _timeProvider);

//        if (courseResult.IsFailure)
//        {
//            return Result.Failure<CreateCourseResponse>(courseResult.Error);
//        }

//        Course course = courseResult.Value;

//        await _dbContext.Courses.AddAsync(course, cancellationToken);
//        await _dbContext.SaveChangesAsync(cancellationToken);

//        return new CreateCourseResponse(course.Id.Value, course.Title.Value);
//    }
//}
