using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseDifficultyChangedDomainEvent(CourseId Id, DifficultyLevel NewDifficulty) : IDomainEvent;
