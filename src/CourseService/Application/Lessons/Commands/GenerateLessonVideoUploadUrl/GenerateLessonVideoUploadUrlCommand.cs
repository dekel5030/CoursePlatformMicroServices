using Courses.Application.Shared.Dtos;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;

public sealed record GenerateLessonVideoUploadUrlCommand(
    ModuleId ModuleId,
    LessonId LessonId,
    string FileName,
    string ContentType) : ICommand<GenerateUploadUrlDto>;
