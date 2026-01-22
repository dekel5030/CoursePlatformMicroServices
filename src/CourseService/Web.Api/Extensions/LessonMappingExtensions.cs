using Courses.Api.Contracts.Lessons;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Abstractions.Hateoas;
using Courses.Application.Actions;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Api.Extensions;

internal static class LessonMappingExtensions
{
    public static LessonSummaryResponse ToApiContract(
        this LessonSummaryDto dto,
        CoursePolicyContext courseContext,
        LinkProvider linkProvider)
    {
        var lessonContext = new LessonPolicyContext(dto.LessonId, dto.Access);

        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Access.ToString(),
            linkProvider.CreateLessonLinks(courseContext, lessonContext, new ModuleId(Guid.NewGuid())));
    }

    public static LessonDetailsResponse ToApiContract(
        this LessonDetailsDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        LessonPolicyContext lessonContext = new(dto.LessonId, dto.Access);

        // Create a minimal course context for links
        var courseContext = new CoursePolicyContext(
            courseId,
            new UserId(Guid.Empty),
            CourseStatus.Draft,
            0);

        return new LessonDetailsResponse(
            courseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Access.ToString(),
            "Draft",
            dto.VideoUrl?.ToString(),
            linkProvider.CreateLessonLinks(courseContext: courseContext, lessonContext: lessonContext, moduleIdParam: dto.ModuleId));
    }

    public static LessonDetailsResponse ToApiContract(
        this LessonDetailsPageDto dto,
        LinkProvider linkProvider,
        ModuleId moduleId,
        LessonId lessonId)
    {
        var courseContext = new CoursePolicyContext(
            dto.CourseId,
            new UserId(Guid.Empty),
            CourseStatus.Draft,
            0);

        var lessonContext = new LessonPolicyContext(lessonId, Enum.Parse<LessonAccess>(dto.Access));

        IReadOnlyCollection<LinkDto> links = linkProvider.CreateLessonLinks(courseContext, lessonContext, moduleId);

        return new LessonDetailsResponse(
            dto.CourseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl,
            dto.Access,
            "Draft",
            dto.VideoUrl,
            links);
    }

    public static LessonDetailsResponse ToApiContract(this LessonDetailsPageDto dto)
    {
        return new LessonDetailsResponse(
            dto.CourseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl,
            dto.Access,
            "Draft",
            dto.VideoUrl,
            dto.Links);
    }
}
