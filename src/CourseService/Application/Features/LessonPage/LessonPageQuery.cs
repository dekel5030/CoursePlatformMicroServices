using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.LessonPage;

public sealed record LessonPageQuery(Guid LessonId) : IQuery<LessonPageDto>;
