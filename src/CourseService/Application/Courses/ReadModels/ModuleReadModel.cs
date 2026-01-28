using Courses.Application.Courses.Dtos;

namespace Courses.Application.Courses.ReadModels;

public sealed record ModuleReadModel
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required int LessonCount { get; init; }
    public required List<LessonReadModel> Lessons { get; init; }

    public ModuleDto ToDto()
    {
        return new ModuleDto
        {
            Id = Id,
            Title = Title,
            Index = Index,
            Duration = Duration,
            LessonCount = LessonCount,
            Lessons = Lessons.ConvertAll(l => l.ToDto()),
            Links = []
        };
    }
}
