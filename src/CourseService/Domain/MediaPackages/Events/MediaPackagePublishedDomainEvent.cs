using Courses.Domain.Courses.Primitives;
using Courses.Domain.MediaPackages.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.MediaPackages.Events;

public sealed record MediaPackagePublishedDomainEvent(
    MediaPackageId Id,
    CourseId CourseId,
    UserId InstructorId,
    IReadOnlyList<RawAsset> Assets,
    string? Message) : IDomainEvent;
