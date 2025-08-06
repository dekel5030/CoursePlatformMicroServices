using CourseService.Models;
using Microsoft.EntityFrameworkCore;
using CourseService.Extentions;
using CourseService.Dtos.Courses;

namespace CourseService.Data.CoursesRepo;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _dbContext;

    public CourseRepository(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddCourseAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course);
    }

    public void DeleteCourse(Course course)
    {
        _dbContext.Courses.Remove(course);
    }

    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        return await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }

    public async Task<IEnumerable<Course>> SearchCoursesAsync(CourseSearchDto query)
    {
        var coursesQuery = _dbContext.Courses.AsQueryable();

        return await coursesQuery.ApplySearchFilters(query).ToListAsync();
    }

    public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
    {
        return await _dbContext.Lessons.FindAsync(lessonId);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId)
    {
        return await _dbContext.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        await _dbContext.Lessons.AddAsync(lesson);
    }

    public void DeleteLesson(Lesson lesson)
    {
        _dbContext.Lessons.Remove(lesson);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
