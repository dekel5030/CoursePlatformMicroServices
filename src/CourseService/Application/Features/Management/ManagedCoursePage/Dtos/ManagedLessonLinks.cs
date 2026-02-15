using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public sealed record ManagedLessonLinks(
    LinkRecord Self,
    LinkRecord? PartialUpdate,
    LinkRecord? UploadVideoUrl,
    LinkRecord? AiGenerate,
    LinkRecord? Move);
