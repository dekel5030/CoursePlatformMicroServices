using Domain.Courses.Primitives;

namespace Domain.Courses;

public class KnownCourse
{
    private KnownCourse() { }

    public ExternalCourseId CourseId { get; private set; } = new ExternalCourseId(string.Empty);
    public string Title { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static KnownCourse Create(ExternalCourseId courseId, string title, bool isActive)
    {
        return new KnownCourse
        {
            CourseId = courseId,
            Title = title,
            IsActive = isActive
        };
    }

    public void Update(string title, bool isActive)
    {
        Title = title;
        IsActive = isActive;
    }
}
