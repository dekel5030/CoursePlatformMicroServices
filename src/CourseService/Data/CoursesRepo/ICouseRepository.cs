using CourseService.Dtos.Courses;
using CourseService.Dtos.Lessons;
using CourseService.Models;

namespace CourseService.Data.CoursesRepo;

public interface ICourseRepository
{
    Task<Course?> GetCourseByIdAsync(int courseId, bool includeLessons);
    Task<(IEnumerable<Course> Courses, int TotalCount)> SearchCoursesAsync(CourseSearchDto query);
    Task AddCourseAsync(Course course);
    void DeleteCourse(Course course);
    Task<bool> CourseExistsAsync(int id);

    Task<Lesson?> GetLessonByIdAsync(int lessonId);
    Task<(IEnumerable<Lesson> Lessons, int TotalCount)> GetLessonsAsync(LessonSearchDto query);
    Task AddLessonAsync(Lesson lesson);
    void DeleteLesson(Lesson lesson);
    Task<int> GetLastLessonOrder(int courseId);
}