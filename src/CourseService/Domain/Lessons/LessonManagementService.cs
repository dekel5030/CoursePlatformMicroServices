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
            await _lessonRepository.AddAsync(lessonResult.Value, cancellationToken);
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

    public async Task<Result> ReorderLessonsAsync(
        ModuleId moduleId,
        List<LessonId> newOrder,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Lesson> lessons = await _lessonRepository
            .ListAsync(l => l.ModuleId == moduleId, cancellationToken);

        if (lessons.Count != newOrder.Count)
        {
            return Result.Failure(Error.Validation(
                "InvalidOrderCount",
                "The number of lessons in the new order does not match the existing lessons."));
        }

        var lessonIdsSet = lessons.Select(l => l.Id).ToHashSet();
        if (newOrder.Any(id => !lessonIdsSet.Contains(id)))
        {
            return Result.Failure(Error.Validation(
                "InvalidLessonIds",
                "The new order contains lesson IDs that do not belong to this module."));
        }

        foreach (Lesson lesson in lessons)
        {
            int newIndex = newOrder.IndexOf(lesson.Id);
            if (newIndex != -1)
            {
                lesson.ChangeIndex(newIndex);
            }
        }

        return Result.Success();
    }

    public async Task<Result> MoveLessonAsync(
        LessonId lessonId,
        ModuleId targetModuleId,
        int targetIndex,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        Module? targetModule = await _moduleRepository.GetByIdAsync(targetModuleId, cancellationToken);
        if (targetModule is null)
        {
            return Result.Failure(ModuleErrors.NotFound);
        }

        if (targetModule.CourseId != lesson.CourseId)
        {
            return Result.Failure(Error.Validation(
                "InvalidTargetModule",
                "The target module must belong to the same course as the lesson."));
        }

        if (lesson.ModuleId == targetModuleId && lesson.Index == targetIndex)
        {
            return Result.Success();
        }

        ModuleId sourceModuleId = lesson.ModuleId;
        var targetLessons = (await _lessonRepository
            .ListAsync(l => l.ModuleId == targetModuleId, cancellationToken))
            .OrderBy(l => l.Index)
            .ToList();

        if (targetIndex < 0 || targetIndex > targetLessons.Count)
        {
            return Result.Failure(Error.Validation(
                "InvalidTargetIndex",
                "The target index is out of range."));
        }

        if (sourceModuleId == targetModuleId)
        {
            var sourceLessonsSameModule = (await _lessonRepository
                .ListAsync(l => l.ModuleId == sourceModuleId, cancellationToken))
                .OrderBy(l => l.Index)
                .ToList();

            var reordered = sourceLessonsSameModule
                .Where(l => l.Id != lessonId)
                .ToList();
            reordered.Insert(targetIndex, lesson);

            for (int i = 0; i < reordered.Count; i++)
            {
                reordered[i].ChangeIndex(i);
            }

            return Result.Success();
        }

        var sourceLessons = (await _lessonRepository
            .ListAsync(l => l.ModuleId == sourceModuleId, cancellationToken))
            .OrderBy(l => l.Index)
            .ToList();

        int oldIndex = lesson.Index;

        for (int i = targetLessons.Count - 1; i >= targetIndex; i--)
        {
            targetLessons[i].ChangeIndex(i + 1);
        }

        lesson.MoveToModule(targetModuleId, targetIndex);

        for (int i = oldIndex + 1; i < sourceLessons.Count; i++)
        {
            Lesson l = sourceLessons[i];
            if (l.Id != lessonId)
            {
                l.ChangeIndex(i - 1);
            }
        }

        return Result.Success();
    }
}
