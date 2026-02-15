using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public sealed record ManagedCourseLinks(
    LinkRecord Self,
    LinkRecord? CoursePage,
    LinkRecord? Analytics,
    LinkRecord? PartialUpdate,
    LinkRecord? Delete,
    LinkRecord? Publish,
    LinkRecord? GenerateImageUploadUrl,
    LinkRecord? CreateModule,
    LinkRecord? ReorderModules);
