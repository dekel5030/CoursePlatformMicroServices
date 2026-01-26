using Courses.Application.Abstractions.Ai;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Commands.GenerateLessonWithAi;

internal sealed class GenerateLessonWithAiCommandHandler
    : ICommandHandler<GenerateLessonWithAiCommand, GenerateLessonWithAiResponse>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IAiProvider<GenerateLessonWithAiResponse> _aiProvider;

    public GenerateLessonWithAiCommandHandler(
        IWriteDbContext dbContext,
        IAiProvider<GenerateLessonWithAiResponse> aiProvider)
    {
        _dbContext = dbContext;
        _aiProvider = aiProvider;
    }

    public async Task<Result<GenerateLessonWithAiResponse>> Handle(
        GenerateLessonWithAiCommand request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .Where(lesson => lesson.Id == request.LessonId
                    && lesson.ModuleId == request.ModuleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<GenerateLessonWithAiResponse>(LessonErrors.NotFound);
        }

        if (!lesson.TranscriptLines.Any())
        {
            return Result.Failure<GenerateLessonWithAiResponse>(
                Error.Validation(
                    "Lesson.NoTranscript",
                    "Cannot update lesson details because transcript lines are empty."));
        }

        string fullTranscript = string.Join(" ", lesson.TranscriptLines.Select(line => line.Text));

        GenerateLessonWithAiResponse response = await SendAiRequest(fullTranscript, cancellationToken);

        return Result.Success(response);
    }

    private Task<GenerateLessonWithAiResponse> SendAiRequest(
        string transcript,
        CancellationToken cancellationToken = default)
    {
        string prompt = """
            You are an AI assistant specialized in educational content analysis and summarization.

            You will receive a full transcript of a lesson (spoken content).
            Your task is to extract and generate the following structured information:

            1. Lesson Title
            2. Lesson Description

            Requirements and constraints:

            - Language:
              - You MUST write the title and description in the SAME language as the original transcript.
              - Do NOT translate unless the transcript itself is translated.

            - Lesson Title:
              - Maximum length: 100 characters (strict).
              - Should be concise, clear, and accurately reflect the core topic of the lesson.
              - Avoid unnecessary filler words.
              - Prefer professional, educational phrasing suitable for a course catalog.

            - Lesson Description:
              - Maximum length: 2000 characters (strict).
              - Should be written in a professional, instructional tone.
              - Summarize the main topics, skills, or concepts covered in the lesson.
              - Clearly describe what the learner will understand or gain from this lesson.
              - Do NOT include timestamps, speaker names, or irrelevant small talk.
              - Do NOT invent topics that are not present in the transcript.

            - Content quality guidelines:
              - Be factual and grounded only in the provided transcript.
              - Organize ideas logically and clearly.
              - Focus on educational value and learning outcomes.
              - Assume the description will be displayed to students before enrolling in the lesson.

            Now analyze the following lesson transcript: \n\n
            """;

        prompt += transcript;

        return _aiProvider.SendAsync(prompt, cancellationToken);
    }
}
