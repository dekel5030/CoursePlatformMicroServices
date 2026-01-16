using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.GenerateThumbnailUploadUrl;

public record GenerateThumbnailUploadUrlCommand(
    CourseId Id,
    string FileName,
    string ContentType) : ICommand<GenerateUploadUrlResponse>;

public record GenerateUploadUrlResponse(
    string UploadUrl,
    string FileKey,
    DateTime ExpiresAt
);
