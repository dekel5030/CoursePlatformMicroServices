using CourseService.Dtos.Courses;
using CourseService.Models;

namespace CourseService.Data.CoursesRepo;

public interface ICourseRepository
{
    Task<Course?> GetCourseByIdAsync(int courseId);
    Task<IEnumerable<Course>> SearchCoursesAsync(CourseSearchDto query);
    Task AddCourseAsync(Course course);
    void DeleteCourse(Course course);

    Task<Lesson?> GetLessonByIdAsync(int lessonId);
    Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId);
    Task AddLessonAsync(Lesson lesson);
    void DeleteLesson(Lesson lesson);

    Task<bool> SaveChangesAsync();
}