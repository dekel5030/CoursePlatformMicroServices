using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.Shared;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetTranscript;

internal sealed class GetTranscriptQueryHandler
    : IQueryHandler<GetTranscriptQuery, IReadOnlyList<TranscriptSegmentDto>>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ITranscriptFileService _transcriptFileService;
    private readonly IUserContext _userContext;

    public GetTranscriptQueryHandler(
        IWriteDbContext writeDbContext,
        ITranscriptFileService transcriptFileService,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _transcriptFileService = transcriptFileService;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<TranscriptSegmentDto>>> Handle(
        GetTranscriptQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _writeDbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(lesson => lesson.Id == lessonId, cancellationToken);

        if (lesson is null || lesson.Transcript is null)
        {
            return LessonErrors.NotFound;
        }

        Course? course = await _writeDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == lesson.CourseId, cancellationToken);

        if (course is null)
        {
            return CourseErrors.NotFound;
        }

        Result<UserId> authResult = InstructorAuthorization.EnsureInstructorAuthorized(
            _userContext,
            course.InstructorId);

        if (authResult.IsFailure)
        {
            return CourseErrors.Unauthorized;
        }

        List<TranscriptSegmentDto> segments =
            await FetchAndParseTranscript(lesson.Transcript.Path, cancellationToken);

        return segments;
    }

    private async Task<List<TranscriptSegmentDto>> FetchAndParseTranscript(
        string transcriptPath, 
        CancellationToken cancellationToken = default)
    {
        string? vttContent = await _transcriptFileService
            .GetVttContentAsync(transcriptPath, StorageCategory.Public, cancellationToken);

        if (string.IsNullOrEmpty(vttContent))
        {
            return [];
        }

        List<TranscriptLine> lines = VttParser.Parse(vttContent);

        var segments = new List<TranscriptSegmentDto>(lines.Count);
        for (int i = 0; i < lines.Count; i++)
        {
            TranscriptLine line = lines[i];
            segments.Add(new TranscriptSegmentDto(
                Id: i,
                StartTime: line.Start.TotalSeconds,
                EndTime: line.End.TotalSeconds,
                Text: line.Text));
        }

        return segments;
    }
}
