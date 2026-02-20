using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.GenerateCourseImageUploadUrl;

public record GenerateCourseImageUploadUrlCommand(
    CourseId Id,
    string FileName,
    string ContentType) : ICommand<UploadUrlDto>;
