using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;

public sealed record GenerateLessonVideoUploadUrlCommand(
    CourseId CourseId,
    LessonId LessonId,
    string FileName,
    string ContentType) : ICommand<GenerateUploadUrlDto>;
