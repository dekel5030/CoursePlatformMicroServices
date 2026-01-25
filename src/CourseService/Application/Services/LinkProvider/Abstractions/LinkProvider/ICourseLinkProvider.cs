using Courses.Application.Shared.Dtos;

namespace Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

public interface ICourseLinkProvider
{
    LinkDto GetSelfLink(Guid courseId);
    LinkDto GetUpdateLink(Guid courseId);
    LinkDto GetDeleteLink(Guid courseId);
    LinkDto GetUploadImageUrlLink(Guid courseId);
    LinkDto GetCreateModuleLink(Guid courseId);
    List<LinkDto> GetPaginationLinks(PagedQueryDto query, int totalCount);
    LinkDto GetCreateCourseLink();
}
