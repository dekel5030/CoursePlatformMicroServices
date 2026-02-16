using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetTranscript;

public sealed record GetTranscriptQuery(Guid LessonId) : IQuery<IReadOnlyList<TranscriptSegmentDto>>;
