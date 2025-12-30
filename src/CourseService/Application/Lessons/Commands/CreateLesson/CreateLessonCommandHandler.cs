//using Courses.Application.Abstractions.Data;
//using Courses.Domain.Courses.Errors;
//using Courses.Domain.Courses.Primitives;
//using Courses.Domain.Lessons;
//using Courses.Domain.Lessons.Primitives;
//using Kernel;
//using Kernel.Messaging.Abstractions;
//using Microsoft.EntityFrameworkCore;

//namespace Courses.Application.Lessons.Commands.CreateLesson;

//public class CreateLessonCommandHandler : ICommandHandler<CreateLessonCommand, LessonId>
//{
//    private readonly IWriteDbContext _dbContext;

//    public CreateLessonCommandHandler(IWriteDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public async Task<Result<LessonId>> Handle(
//        CreateLessonCommand request,
//        CancellationToken cancellationToken = default)
//    {
//        CreateLessonDto dto = request.Dto;

//        var courseId = new CourseId(dto.CourseId);
//        var courseExists = await _dbContext.Courses
//            .AnyAsync(c => c.Id == courseId, cancellationToken);

//        if (!courseExists)
//        {
//            return Result.Failure<LessonId>(CourseErrors.NotFound);
//        }

//        var lesson = Lesson.CreateLesson(
//            dto.Title,
//            dto.Description,
//            dto.VideoUrl,
//            dto.ThumbnailImage,
//            dto.IsPreview,
//            dto.Order,
//            dto.Duration
//        );

//        lesson.CourseId = courseId;

//        await _dbContext.Lessons.AddAsync(lesson, cancellationToken);
//        await _dbContext.SaveChangesAsync(cancellationToken);

//        return Result.Success(lesson.Id);
//    }
//}