using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules.Errors;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Modules;

public sealed class ModuleManagementService
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;

    public ModuleManagementService(
        IModuleRepository moduleRepository,
        ICourseRepository courseRepository)
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Result<Module>> CreateModuleAsync(
        CourseId courseId,
        Title title,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return Result.Failure<Module>(CourseErrors.NotFound);
        }

        IReadOnlyList<Module> existingModules = await _moduleRepository
            .ListAsync(m => m.CourseId == courseId, cancellationToken);

        int nextIndex = existingModules.Any() ? existingModules.Max(m => m.Index) + 1 : 0;

        Result<Module> moduleResult = Module.Create(courseId, nextIndex, title);

        if (moduleResult.IsSuccess)
        {
            await _moduleRepository.AddAsync(moduleResult.Value, cancellationToken);
        }

        return moduleResult;
    }

    public async Task<Result> RemoveModuleAsync(ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        Module? moduleToRemove = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
        if (moduleToRemove is null)
        {
            return Result.Failure(ModuleErrors.NotFound);
        }

        int removedIndex = moduleToRemove.Index;
        CourseId courseId = moduleToRemove.CourseId;

        _moduleRepository.Remove(moduleToRemove);
        moduleToRemove.Delete();

        IReadOnlyList<Module> modulesToShift = await _moduleRepository
            .ListAsync(m => m.CourseId == courseId && m.Index > removedIndex, cancellationToken);

        foreach (Module module in modulesToShift)
        {
            module.UpdateIndex(module.Index - 1);
        }

        return Result.Success();
    }

    public async Task<Result> ReorderModulesAsync(
        CourseId courseId,
        List<ModuleId> newOrder,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Module> modules = await _moduleRepository.ListAsync(m => m.CourseId == courseId, cancellationToken);

        if (modules.Count != newOrder.Count)
        {
            return Result.Failure(Error.Validation(
                "InvalidOrderCount",
                "The number of modules in the new order does not match the existing modules."));
        }

        foreach (Module module in modules)
        {
            int newIndex = newOrder.IndexOf(module.Id);
            if (newIndex != -1)
            {
                module.UpdateIndex(newIndex);
            }
        }

        return Result.Success();
    }
}
