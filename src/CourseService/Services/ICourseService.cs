using Common;
using CourseService.Dtos;
using CourseService.Dtos.Courses;
using CourseService.Dtos.Lessons;

namespace CourseService.Services;

public interface ICourseService
{
    Task<Result<CourseReadDto>> AddCourseAsync(CourseCreateDto course);
    Task<Result<CourseReadDto>> GetCourseByIdAsync(int courseId, bool includeLessons);
    Task<PagedResponseDto<CourseReadDto>> SearchCoursesAsync(CourseSearchDto query);
    Task<Result<CourseReadDto>> DeleteCourseAsync(int courseId);

    Task<Result<LessonReadDto>> AddLessonAsync(LessonCreateDto lesson);
    Task<Result<LessonReadDto>> GetLessonByIdAsync(int lessonId);
    Task<PagedResponseDto<LessonReadDto>> GetLessonsAsync(LessonSearchDto query);
    Task<Result<LessonReadDto>> DeleteLessonAsync(int lessonId);
}