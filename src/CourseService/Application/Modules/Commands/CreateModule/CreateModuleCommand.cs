using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.CreateModule;

public sealed record CreateModuleCommand(
    CourseId CourseId,
    Title? Title) : ICommand<CreateModuleResponse>;
