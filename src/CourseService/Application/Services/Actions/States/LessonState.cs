using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Services.Actions.States;

public sealed record LessonState(LessonId Id, LessonAccess LessonAccess);
