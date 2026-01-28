using Courses.Application.Courses.Dtos;

namespace Courses.Application.Courses.ReadModels;

public sealed class ModuleReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public TimeSpan Duration { get; set; }
    public int LessonCount { get; set; }
    public List<LessonReadModel> Lessons { get; private set; } = new();

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
