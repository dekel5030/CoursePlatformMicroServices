namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseStatsReadModel
{
    public Guid CourseId { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public int LessonsCount { get; set; }
    public int ModulesCount { get; set; }
    public int EnrollmentCount { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public Dictionary<Guid, TimeSpan> LessonDurations { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only

    public void AddLesson(Guid lessonId, TimeSpan duration)
    {
        if (LessonDurations.ContainsKey(lessonId))
        {
            return;
        }

        LessonDurations[lessonId] = duration;
        TotalDuration += duration;
        LessonsCount++;
    }

    public void UpdateLessonDuration(Guid lessonId, TimeSpan newDuration)
    {
        if (!LessonDurations.TryGetValue(lessonId, out TimeSpan oldDuration))
        {
            return;
        }

        TimeSpan durationDiff = newDuration - oldDuration;
        LessonDurations[lessonId] = newDuration;
        TotalDuration += durationDiff;
    }

    public void RemoveLesson(Guid lessonId)
    {
        if (!LessonDurations.Remove(lessonId, out TimeSpan duration))
        {
            return;
        }

        TotalDuration -= duration;
        LessonsCount--;
    }

    public void IncrementModules()
    {
        ModulesCount++;
    }

    public void DecrementModules()
    {
        ModulesCount--;
    }
}
