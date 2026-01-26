namespace Courses.Domain.Lessons.Primitives;

public sealed record TranscriptLine(TimeSpan Start, TimeSpan End, string Text);
