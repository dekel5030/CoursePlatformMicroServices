using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Lessons.Queries.GetTranscript;

public sealed record TranscriptSegmentDto(
    int Id,
    double StartTime,
    double EndTime,
    string Text);


public sealed record TranscriptDto(
    IReadOnlyList<TranscriptSegmentDto> Segments,
    LinkRecord Update);
