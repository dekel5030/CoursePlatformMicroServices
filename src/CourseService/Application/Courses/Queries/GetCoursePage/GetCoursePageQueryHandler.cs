using Courses.Application.Categories.Dtos;
using Courses.Application.Categories.Queries.GetCategories;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Lessons.Queries.GetLessons;
using Courses.Application.Modules.Queries.GetModules;
using Courses.Application.Users.Queries.GetUsers;
using Courses.Domain.Categories.Errors;
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

        Result<CourseDto> courseResult = await SendInScopeAsync(new GetCourseByIdQuery(courseId), cancellationToken);
        if (courseResult.IsFailure)
        {
            return Result.Failure<CoursePageDto>(courseResult.Error);
        }

        CourseDto course = courseResult.Value;
        Guid instructorId = course.InstructorId;
        Guid categoryId = course.CategoryId;

        Task<Result<IReadOnlyList<ModuleDto>>> modulesTask = SendInScopeAsync(
            new GetModulesQuery(new ModuleFilter(CourseId: courseId)), cancellationToken);
        Task<Result<IReadOnlyList<LessonDto>>> lessonsTask = SendInScopeAsync(
            new GetLessonsQuery(new LessonFilter(CourseId: courseId)), cancellationToken);
        Task<Result<IReadOnlyList<UserDto>>> instructorTask = SendInScopeAsync(
            new GetUsersQuery(new UserFilter(Ids: [instructorId])), cancellationToken);
        Task<Result<IReadOnlyList<CategoryDto>>> categoryTask = SendInScopeAsync(
            new GetCategoriesQuery(new CategoryFilter(Ids: [categoryId])), cancellationToken);
        await Task.WhenAll(modulesTask, lessonsTask, instructorTask, categoryTask);

        return await AssembleCoursePageAsync(course, modulesTask, lessonsTask, instructorTask, categoryTask);
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
        CourseDto course,
        Task<Result<IReadOnlyList<ModuleDto>>> modulesTask,
        Task<Result<IReadOnlyList<LessonDto>>> lessonsTask,
        Task<Result<IReadOnlyList<UserDto>>> instructorTask,
        Task<Result<IReadOnlyList<CategoryDto>>> categoryTask)
    {
        Result<IReadOnlyList<ModuleDto>> modulesResult = await modulesTask;
        Result<IReadOnlyList<LessonDto>> lessonsResult = await lessonsTask;
        Result<IReadOnlyList<UserDto>> instructorsResult = await instructorTask;
        Result<IReadOnlyList<CategoryDto>> categoryResult = await categoryTask;

        if (categoryResult.IsFailure
            || modulesResult.IsFailure
            || lessonsResult.IsFailure
            || instructorsResult.IsFailure)
        {
            return Result.Failure<CoursePageDto>(new ValidationError([
                categoryResult.Error
                ?? modulesResult.Error
                ?? lessonsResult.Error
                ?? instructorsResult.Error!]));
        }

        CategoryDto? category = categoryResult.Value.Count > 0 ? categoryResult.Value[0] : null;
        if (category is null)
        {
            return Result.Failure<CoursePageDto>(CategoryErrors.NotFound);
        }

        var response = new CoursePageDto
        {
            Course = course,
            Modules = modulesResult.Value.ToDictionary(m => m.Id),
            Lessons = lessonsResult.Value.ToDictionary(l => l.Id),
            Instructors = instructorsResult.Value.ToDictionary(i => i.Id),
            Categories = new Dictionary<Guid, CategoryDto> { [category.Id] = category }
        };

        return Result.Success(response);
    }
}
