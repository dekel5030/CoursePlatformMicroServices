namespace Courses.Domain.EditingJob;

public sealed record EditingJobId(Guid Value)
{
    public static EditingJobId NewId() => new(Guid.CreateVersion7());
}
