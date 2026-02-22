using Kernel;

namespace Courses.Domain.MediaProcessingTask.Errors;

public static class EditingJobErrors
{     
    public static Error JobAlreadyAssigned => Error.Validation("Job.AlreadyAssigned", "Job is not in pending state.");
    public static Error JobNotInProgress => Error.Validation("Job.NotInProgress", "Cannot complete a job that is not in progress.");
}
