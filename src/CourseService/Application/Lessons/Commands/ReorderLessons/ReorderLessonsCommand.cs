using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.ReorderLessons;

public sealed record ReorderLessonsCommand(ModuleId ModuleId, List<LessonId> LessonIds) : ICommand;
