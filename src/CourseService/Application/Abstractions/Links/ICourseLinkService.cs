using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Application.Abstractions.Links;

public interface ICourseLinkService
{
    LinkDto GetSelfLink(Guid courseId);
    LinkDto GetUpdateLink(Guid courseId);
    LinkDto GetDeleteLink(Guid courseId);
    LinkDto GetUploadImageUrlLink(Guid courseId);
    LinkDto GetCreateModuleLink(Guid courseId);
}
