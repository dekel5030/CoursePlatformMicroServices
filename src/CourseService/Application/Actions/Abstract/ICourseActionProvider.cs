using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;

namespace Courses.Application.Actions.Abstract;

public interface ICourseActionProvider
{
    IReadOnlyCollection<CourseAction> GetAllowedActions(Course course);
    IReadOnlyCollection<LessonAction> GetAllowedActions(Course course, Lesson lesson);
    IReadOnlyCollection<CourseCollectionAction> GetAllowedCollectionActions();
}
