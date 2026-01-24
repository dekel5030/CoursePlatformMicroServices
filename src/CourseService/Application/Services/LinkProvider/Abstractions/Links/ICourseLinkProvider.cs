namespace Courses.Application.Services.LinkProvider.Abstractions.Links;

public interface ICourseLinkProvider
{
    LinkDto GetSelfLink(Guid courseId);
    LinkDto GetUpdateLink(Guid courseId);
    LinkDto GetDeleteLink(Guid courseId);
    LinkDto GetUploadImageUrlLink(Guid courseId);
    LinkDto GetCreateModuleLink(Guid courseId);
}
