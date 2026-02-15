using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public sealed record ManagedModuleLinks(
    LinkRecord? CreateLesson,
    LinkRecord? PartialUpdate,
    LinkRecord? Delete,
    LinkRecord? ReorderLessons);
