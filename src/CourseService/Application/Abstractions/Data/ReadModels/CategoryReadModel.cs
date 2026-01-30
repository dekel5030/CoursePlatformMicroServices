namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CategoryReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
