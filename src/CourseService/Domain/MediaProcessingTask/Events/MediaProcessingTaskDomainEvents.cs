using Courses.Domain.Lessons.Primitives;
using Courses.Domain.MediaProcessingTask.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.MediaProcessingTask.Events;

public sealed record MediaProcesingTaskCompletedDomainEvent(
    TaskId TaskId,
    LessonId OriginalLessonId,
    string Message,
    IReadOnlyList<Url> RawResources,
    IReadOnlyList<Url> OutputResources) : IDomainEvent;
