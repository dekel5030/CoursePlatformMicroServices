using Courses.Domain.Courses.Primitives;
using Courses.Domain.CourseViews.Primitives;
using Courses.Domain.Shared;

namespace Courses.Domain.CourseViews;

public class CourseView : Entity<CourseViewId>
{
    public override CourseViewId Id { get; protected set; }
    public CourseId CourseId { get; private set; }
    public UserId? UserId { get; private set; }
    public DateTimeOffset ViewedAt { get; private set; }

#pragma warning disable S1133
#pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private CourseView() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    private CourseView(CourseViewId id, CourseId courseId, UserId? userId, DateTimeOffset viewedAt)
    {
        Id = id;
        CourseId = courseId;
        UserId = userId;
        ViewedAt = viewedAt;
    }

    public static CourseView Create(CourseId courseId, UserId? userId, DateTimeOffset viewedAt)
    {
        return new CourseView(CourseViewId.CreateNew(), courseId, userId, viewedAt);
    }
}
