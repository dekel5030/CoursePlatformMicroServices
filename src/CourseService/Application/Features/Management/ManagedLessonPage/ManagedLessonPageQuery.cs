using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.ManagedLessonPage;

public sealed record ManagedLessonPageQuery(Guid LessonId) : IQuery<ManagedLessonPageDto>;
