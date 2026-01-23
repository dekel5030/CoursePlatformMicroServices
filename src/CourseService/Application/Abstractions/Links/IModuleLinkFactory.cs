using Courses.Application.Abstractions.LinkProvider;
using Courses.Domain.Module;

namespace Courses.Application.Abstractions.Links;

public interface IModuleLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Module module);
}
