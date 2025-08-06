using CourseService.Models;
using Microsoft.EntityFrameworkCore;
using CourseService.Extentions;
using CourseService.Dtos.Courses;
using System.Threading.Tasks;
using CourseService.Dtos.Lessons;

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

    public async Task<Course?> GetCourseByIdAsync(int courseId, bool includeLessons = false)
    {
        if (includeLessons)
        {
            return await _dbContext.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }
        else
        {
            return await _dbContext.Courses.FindAsync(courseId);
        }
    }

    public async Task<(IEnumerable<Course> Courses, int TotalCount)> SearchCoursesAsync(CourseSearchDto query)
    {
        var filteredQuery = _dbContext.Courses
            .ApplySearchFilters(query);

        var totalCount = await filteredQuery.CountAsync();

        var items = await filteredQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> CourseExistsAsync(int courseId)
    {
        return await _dbContext.Courses.AnyAsync(c => c.Id == courseId);
    }

    public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
    {
        return await _dbContext.Lessons.FindAsync(lessonId);
    }

    public async Task<(IEnumerable<Lesson> Lessons, int TotalCount)> GetLessonsAsync(LessonSearchDto query)
    {
        var baseQuery = _dbContext.Lessons
            .Where(l => l.CourseId == query.CourseId);

        var totalCount = await baseQuery.CountAsync();

        var lessons = await baseQuery
            .OrderBy(l => l.Order)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (lessons, totalCount);
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        await _dbContext.Lessons.AddAsync(lesson);
    }

    public void DeleteLesson(Lesson lesson)
    {
        _dbContext.Lessons.Remove(lesson);
    }

    public async Task<int> GetLastLessonOrder(int courseId)
    {
        return await _dbContext.Lessons
                        .Where(l => l.CourseId == courseId)
                        .Select(l => l.Order)
                        .DefaultIfEmpty(0)
                        .MaxAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
