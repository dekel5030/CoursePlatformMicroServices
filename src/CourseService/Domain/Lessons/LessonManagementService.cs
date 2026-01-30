using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Errors;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Lessons;

public sealed class LessonManagementService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IModuleRepository _moduleRepository;

    public LessonManagementService(
        ILessonRepository lessonRepository,
        IModuleRepository moduleRepository)
    {
        _lessonRepository = lessonRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<Result<Lesson>> CreateLessonAsync(
        CourseId courseId,
        ModuleId moduleId,
        Title title,
        Description description,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
        if (module is null)
        {
            return Result.Failure<Lesson>(ModuleErrors.NotFound);
        }

        IReadOnlyList<Lesson> existingLessons = await _lessonRepository
            .ListAsync(l => l.ModuleId == moduleId, cancellationToken);

        int nextIndex = existingLessons.Any()
            ? existingLessons.Max(l => l.Index) + 1
            : 0;

        Result<Lesson> lessonResult = Lesson.Create(courseId, moduleId, title, description, nextIndex);

        if (lessonResult.IsSuccess)
        {
            _lessonRepository.Add(lessonResult.Value);
        }

        return lessonResult;
    }

    public async Task<Result> RemoveLessonAsync(LessonId lessonId, CancellationToken cancellationToken = default)
    {
        Lesson? lessonToRemove = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lessonToRemove is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        IReadOnlyList<Lesson> allLessonsInModule = await _lessonRepository
            .ListAsync(l => l.ModuleId == lessonToRemove.ModuleId, cancellationToken);

        var orderedLessons = allLessonsInModule
            .Where(l => l.Id != lessonId)
            .OrderBy(l => l.Index)
            .ToList();

        for (int i = 0; i < orderedLessons.Count; i++)
        {
            orderedLessons[i].ChangeIndex(i);
        }

        _lessonRepository.Remove(lessonToRemove);
        lessonToRemove.Delete();

        return Result.Success();
    }
}
