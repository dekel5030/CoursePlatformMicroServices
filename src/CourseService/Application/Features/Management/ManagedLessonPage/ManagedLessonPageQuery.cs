using Courses.Application.Lessons.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.ManagedLessonPage;

public sealed record ManagedLessonPageQuery(Guid LessonId) : IQuery<LessonDetailsPageDto>;
