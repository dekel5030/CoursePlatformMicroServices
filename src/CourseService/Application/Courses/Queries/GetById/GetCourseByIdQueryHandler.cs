using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCoursePage;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IMediator _mediator;

    public GetCourseByIdQueryHandler(
        ILinkBuilderService linkBuilder,
        IMediator mediator)
    {
        _linkBuilder = linkBuilder;
        _mediator = mediator;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var innerQuery = new GetCoursePageQuery(request.Id.Value);
        Result<CoursePageDto> innerQueryResult = await _mediator.Send(innerQuery, cancellationToken);

        if (innerQueryResult.IsFailure)
        {
            return innerQueryResult;
        }

        CoursePageDto dto = innerQueryResult.Value;
        dto.EnrichWithLinks(_linkBuilder);

        return Result.Success(dto);
    }
}

internal static class DtoEnrichmentExtensions
{
    public static void EnrichWithLinks(this CoursePageDto dto, ILinkBuilderService linkBuilder)
    {
        var courseState = new CourseState(new CourseId(dto.Id), new UserId(dto.InstructorId), dto.Status);
        dto.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKeys.Course, courseState));
        foreach (ModuleDto module in dto.Modules)
        {
            var moduleState = new ModuleState(new ModuleId(module.Id));
            var moduleContext = new ModuleLinkContext(courseState, moduleState);
            module.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKeys.Module, moduleContext));
            foreach (LessonDto lesson in module.Lessons)
            {
                var lessonState = new LessonState(new LessonId(lesson.Id), lesson.Access);
                var lessonContext = new LessonLinkContext(courseState, moduleState, lessonState, null);
                lesson.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKeys.Lesson, lessonContext));
            }
        }
    }
}
