namespace Courses.Application.Modules.Commands.CreateModule;

public sealed record CreateModuleResponse(Guid ModuleId, Guid CourseId, string Title);
