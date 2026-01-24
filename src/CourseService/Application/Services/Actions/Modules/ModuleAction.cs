namespace Courses.Application.Services.Actions.Modules;

public sealed record ModuleAction
{
    public string Value { get; }
    private ModuleAction(string value) => Value = value;

    public static readonly ModuleAction Update = new("Edit");
    public static readonly ModuleAction Publish = new("Publish");
    public static readonly ModuleAction CreateLesson = new("CreateLesson");
    public static readonly ModuleAction Delete = new("Delete");
    public static readonly ModuleAction Read = new("Read");
    public static readonly ModuleAction UploadImageUrl = new("UploadImage");
}
