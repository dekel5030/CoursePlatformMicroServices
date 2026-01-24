namespace Courses.Application.Modules.Dtos;

public record ModuleSummaryDto(
    Guid Id,
    string Title,
    int Index,
    int LessonCount
);
