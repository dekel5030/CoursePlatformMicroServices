using Courses.Domain.Module;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

public interface IModuleLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Module module);
}
