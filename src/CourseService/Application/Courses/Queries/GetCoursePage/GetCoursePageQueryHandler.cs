using Courses.Application.Categories.Dtos;
using Courses.Application.Categories.Queries.GetCategoryByCourseId;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Lessons.Queries.GetByCourseId;
using Courses.Application.Modules.Queries.GetByCourseId;
using Courses.Application.Users.Queries.GetInstructorsByCourseId;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal sealed class GetCoursePageQueryHandler
    : IQueryHandler<GetCoursePageQuery, CoursePageDto>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GetCoursePageQueryHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<Result<CoursePageDto>> Handle(
    GetCoursePageQuery request,
    CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        Task<Result<CourseDto>> courseTask = SendInScopeAsync(new GetCourseByIdQuery(courseId), cancellationToken);
        Task<Result<IReadOnlyList<ModuleDto>>> modulesTask = SendInScopeAsync(new GetModulesByCourseIdQuery(courseId), cancellationToken);
        Task<Result<IReadOnlyList<LessonDto>>> lessonsTask = SendInScopeAsync(new GetLessonsByCourseIdQuery(courseId), cancellationToken);
        Task<Result<IReadOnlyList<UserDto>>> instructorTask = SendInScopeAsync(new GetInstructorsByCourseIdQuery(courseId), cancellationToken);
        Task<Result<CategoryDto>> categoryTask = SendInScopeAsync(new GetCategoryByCourseIdQuery(courseId), cancellationToken);
        await Task.WhenAll(courseTask, modulesTask, lessonsTask, instructorTask, categoryTask);

        return await AssembleCoursePageAsync(courseTask, modulesTask, lessonsTask, instructorTask, categoryTask);
    }

    private async Task<Result<TResponse>> SendInScopeAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken ct)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await scopedMediator.Send(query, ct);
    }

    private static async Task<Result<CoursePageDto>> AssembleCoursePageAsync(
        Task<Result<CourseDto>> courseTask,
        Task<Result<IReadOnlyList<ModuleDto>>> modulesTask,
        Task<Result<IReadOnlyList<LessonDto>>> lessonsTask,
        Task<Result<IReadOnlyList<UserDto>>> instructorTask,
        Task<Result<CategoryDto>> categoryTask)
    {
        Result<CourseDto> courseResult = await courseTask;
        Result<IReadOnlyList<ModuleDto>> modulesResult = await modulesTask;
        Result<IReadOnlyList<LessonDto>> lessonsResult = await lessonsTask;
        Result<IReadOnlyList<UserDto>> instructorsResult = await instructorTask;
        Result<CategoryDto> categoryResult = await categoryTask;

        if (categoryResult.IsFailure
            || courseResult.IsFailure
            || modulesResult.IsFailure
            || lessonsResult.IsFailure
            || instructorsResult.IsFailure)
        {
            return Result.Failure<CoursePageDto>(new ValidationError([
                categoryResult.Error
                ?? courseResult.Error
                ?? modulesResult.Error
                ?? lessonsResult.Error
                ?? instructorsResult.Error!]));
        }

        var response = new CoursePageDto
        {
            Course = courseResult.Value,
            Modules = modulesResult.Value.ToDictionary(m => m.Id),
            Lessons = lessonsResult.Value.ToDictionary(l => l.Id),
            Instructors = instructorsResult.Value.ToDictionary(i => i.Id),
            Categories = new Dictionary<Guid, CategoryDto> { [categoryResult.Value.Id] = categoryResult.Value }
        };

        return Result.Success(response);
    }
}
