namespace Courses.Domain.Lessons.Primitives;

public sealed record TranscriptLine
{
    public TimeSpan Start { get; init; }
    public TimeSpan End { get; init; }
    public string Text { get; init; } = string.Empty;
}
