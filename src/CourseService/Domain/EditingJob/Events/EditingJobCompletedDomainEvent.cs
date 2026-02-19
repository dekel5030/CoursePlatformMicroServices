using Courses.Domain.Courses.Primitives;
using Courses.Domain.EditingJob.Primitives;
using Courses.Domain.MediaPackages.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.EditingJob.Events;

public sealed record EditingJobCompletedDomainEvent(
    EditingJobId EditingJobId,
    MediaPackageId MediaPackageId,
    CourseId CourseId,
    string Message) : IDomainEvent;
