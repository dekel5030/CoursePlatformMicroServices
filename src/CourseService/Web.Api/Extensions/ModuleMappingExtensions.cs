using Courses.Api.Contracts.Lessons;
using Courses.Api.Contracts.Modules;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Actions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Api.Extensions;

internal static class ModuleMappingExtensions
{
    public static ModuleDetailsResponse ToApiContract(
        this ModuleDetailsDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        // Map lessons - Note: LessonSummaryDto doesn't have Status, so we'll use a default
        var lessonResponses = dto.Lessons
            .Select(lesson => lesson.ToApiContractForModule(courseId, linkProvider))
            .ToList();

        return new ModuleDetailsResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.Index,
            dto.LessonCount,
            dto.TotalDuration,
            lessonResponses,
            linkProvider.CreateModuleLinks(courseId, dto.Id));
    }

    public static PagedResponse<ModuleDetailsResponse> ToApiContract(
        this ModuleCollectionDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        return new PagedResponse<ModuleDetailsResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract(courseId, linkProvider)).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = linkProvider.CreateModuleCollectionLinks(courseId)
        };
    }

    private static LessonSummaryResponse ToApiContractForModule(
        this LessonSummaryDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        var lessonContext = new LessonPolicyContext(dto.LessonId, dto.Access);
        
        var courseContext = new CoursePolicyContext(
            courseId,
            new Domain.Courses.Primitives.UserId(Guid.Empty),
            Domain.Courses.Primitives.CourseStatus.Draft,
            0);

        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Access.ToString(),
            linkProvider.CreateLessonLinks(courseContext, lessonContext, new Domain.Module.Primitives.ModuleId(Guid.NewGuid())));
    }
}
