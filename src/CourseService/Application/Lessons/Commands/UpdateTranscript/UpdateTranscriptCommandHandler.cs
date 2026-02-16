using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.Shared;
using Courses.Application.Lessons.Queries.GetTranscript;
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

namespace Courses.Application.Lessons.Commands.UpdateTranscript;

internal sealed class UpdateTranscriptCommandHandler : ICommandHandler<UpdateTranscriptCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ITranscriptFileService _transcriptFileService;
    private readonly IUserContext _userContext;

    public UpdateTranscriptCommandHandler(
        IWriteDbContext writeDbContext,
        ITranscriptFileService transcriptFileService,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _transcriptFileService = transcriptFileService;
        _userContext = userContext;
    }

    public async Task<Result> Handle(
        UpdateTranscriptCommand request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _writeDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson is null || lesson.Transcript is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        Course? course = await _writeDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == lesson.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization.EnsureInstructorAuthorized(
            _userContext,
            course.InstructorId);

        if (authResult.IsFailure)
        {
            return Result.Failure(CourseErrors.Unauthorized);
        }

        Result saveResult = await SaveTrancript(
            request.Segments,
            lessonId,
            lesson.Transcript.Path,
            cancellationToken);

        return saveResult;
    }

    private async Task<Result> SaveTrancript(
        IReadOnlyList<TranscriptSegmentDto> segments,
        LessonId lessonId,
        string transcriptPath,
        CancellationToken cancellationToken)
    {
        var lines = new List<TranscriptLine>(segments.Count);
        foreach (TranscriptSegmentDto seg in segments.OrderBy(segment => segment.Id))
        {
            lines.Add(new TranscriptLine(
                TimeSpan.FromSeconds(seg.StartTime),
                TimeSpan.FromSeconds(seg.EndTime),
                seg.Text ?? ""));
        }

        string trancript = VttSerializer.Serialize(lines);

        Result saveResult = await _transcriptFileService
            .SaveVttContentAsync(
            transcriptPath,
            trancript,
            lessonId.ToString(),
            "Transcript",
            StorageCategory.Public,
            cancellationToken);
       
        return saveResult;
    }
}
