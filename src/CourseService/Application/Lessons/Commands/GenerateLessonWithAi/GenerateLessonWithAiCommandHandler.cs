using Courses.Application.Abstractions.Ai;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Shared.Primitives;
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
        You are a senior Instructional Designer and Educational Content Strategist for professional online courses.

        Your objective is to transform a raw lesson transcript into a clear, structured, and high-quality lesson overview
        that is suitable for display in a professional course platform.

        ---

        ### INPUT
        A full verbatim transcript of a single lesson.

        ---

        ### TASK
        Generate:
        1. Lesson Title
        2. Lesson Description

        Base your output ONLY on the content explicitly present in the transcript.

        ---

        ### LESSON TITLE GUIDELINES
        - Maximum length: 100 characters (strict).
        - Professional and clear (not clickbait).
        - Focus on the primary learning outcome or core concept.
        - Avoid vague phrases (e.g., "Introduction to", "Overview of") unless clearly justified by the transcript.

        ---

        ### LESSON DESCRIPTION STRUCTURE (MANDATORY)
        The description MUST be returned as a **single string** containing the following sections, formatted with double newlines between them. 

        **IMPORTANT**: Translate the section headers below into the SAME language as the transcript:

        1. [Short Hook Header]
           - 1–2 concise sentences explaining why this lesson matters.
           - Clearly state the value or problem it addresses.

        2. [What You Will Learn Header]
           - A bulleted list using the "•" character.
           - Exactly 3–5 bullet points.
           - Each bullet must start with a strong action verb (e.g., Understand, Apply, Configure, Analyze).
           - Include only concepts actually taught in the lesson.

        3. [Detailed Summary Header]
           - One well-structured paragraph.
           - Expand on the technical or conceptual topics covered.
           - Remove all filler speech, repetitions, and informal talk.
           - Do not include timestamps or speaker references.

        4. [Outcome Header]
           - One clear, outcome-oriented sentence.
           - Describe what the learner will be able to do after completing this lesson.

        ---

        ### CONSTRAINTS
        - **Language**: Use the SAME language as the transcript for EVERYTHING (content and headers).
        - **Accuracy**: Do NOT add, infer, or assume content that does not appear in the transcript.
        - **Tone**: Professional, instructional, and confidence-building.
        - **Formatting**:
          - Use Markdown for **bold** emphasis.
          - Use <kbd> tags for keyboard shortcuts when mentioned.
        - **Length**: Maximum 2000 characters total (strict).

        ---

        Before finalizing the output, internally verify:
        - The structure matches the required format exactly.
        - All section headers are translated to the transcript's language.
        - The title and description respect the length limits.
        """;

        string finalPrompt = prompt + "\n\nTranscript:\n" + transcript;

        return _aiProvider.SendAsync(finalPrompt, cancellationToken);
    }
}
