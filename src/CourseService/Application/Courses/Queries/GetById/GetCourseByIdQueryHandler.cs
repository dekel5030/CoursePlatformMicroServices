using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCoursePage;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly ICourseLinkFactory _courseLinkFactory;
    private readonly IModuleLinkFactory _moduleLinkFactory;
    private readonly ILessonLinkFactory _lessonLinkFactory;
    private readonly IMediator _mediator;

    public GetCourseByIdQueryHandler(
        ICourseLinkFactory courseLinkFactory,
        IModuleLinkFactory moduleLinkFactory,
        ILessonLinkFactory lessonLinkFactory,
        IMediator mediator)
    {
        _courseLinkFactory = courseLinkFactory;
        _moduleLinkFactory = moduleLinkFactory;
        _lessonLinkFactory = lessonLinkFactory;
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
        dto.EnrichWithLinks(_courseLinkFactory, _moduleLinkFactory, _lessonLinkFactory);

        return Result.Success(dto);
    }
}

internal static class DtoEnrichmentExtensions
{
    public static void EnrichWithLinks(
        this CoursePageDto dto,
        ICourseLinkFactory courseLinkFactory,
        IModuleLinkFactory moduleLinkFactory,
        ILessonLinkFactory lessonLinkFactory)
    {
        var courseState = new CourseState(new CourseId(dto.Id), new UserId(dto.InstructorId), dto.Status);
        dto.Links.AddRange(courseLinkFactory.CreateLinks(courseState));
        foreach (ModuleDto module in dto.Modules)
        {
            var moduleState = new ModuleState(new ModuleId(module.Id));
            module.Links.AddRange(moduleLinkFactory.CreateLinks(courseState, moduleState));
            foreach (LessonDto lesson in module.Lessons)
            {
                var lessonState = new LessonState(new LessonId(lesson.Id), lesson.Access);
                lesson.Links.AddRange(lessonLinkFactory.CreateLinks(courseState, moduleState, lessonState));
            }
        }
    }
}
