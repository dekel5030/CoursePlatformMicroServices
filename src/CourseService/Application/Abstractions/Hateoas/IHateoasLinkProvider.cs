using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Abstractions.Hateoas;

public interface IHateoasLinkProvider
{
    LinkDto Create(string endpointName, string rel, string method, object? values = null);
    List<LinkDto> CreateCategoryCollectionLinks();
    List<LinkDto> CreateCategoryLinks(CategoryId _);
    List<LinkDto> CreateCourseCollectionLinks(CourseCollectionDto courseCollection, PagedQueryDto originalQuery);
    List<LinkDto> CreateCourseLinks(CoursePolicyContext courseContext);
    List<LinkDto> CreateLessonLinks(CoursePolicyContext courseContext, LessonPolicyContext lessonContext, ModuleId moduleIdParam);
    List<LinkDto> CreateModuleCollectionLinks(CourseId courseId);
    List<LinkDto> CreateModuleLinks(CourseId courseId, ModuleId moduleId);
}
