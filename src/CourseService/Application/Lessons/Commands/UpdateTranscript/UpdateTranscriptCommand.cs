using Courses.Application.Lessons.Queries.GetTranscript;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.UpdateTranscript;

public sealed record UpdateTranscriptCommand(
    Guid LessonId,
    IReadOnlyList<TranscriptSegmentDto> Segments) : ICommand;
