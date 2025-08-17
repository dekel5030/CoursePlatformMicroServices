using AutoMapper;
using Common;
using Common.Errors;
using CourseService.Data.CoursesRepo;
using CourseService.Dtos;
using CourseService.Dtos.CourseEvents;
using CourseService.Dtos.Courses;
using CourseService.Dtos.Lessons;
using CourseService.Messaging.Publisher;
using CourseService.Models;

namespace CourseService.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICourseEventPublisher _publisher;

    public CourseService(ICourseRepository repository, IMapper mapper, ICourseEventPublisher publisher)
    {
        _repository = repository;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<Result<CourseReadDto>> AddCourseAsync(CourseCreateDto courseCreateDto)
    {
        var course = CreateCourse(courseCreateDto);

        await _repository.AddCourseAsync(course);
        await _repository.SaveChangesAsync();
        await _publisher.PublishCourseUpsertedEvent(new CourseUpsertedEventDto
        {
            CourseId = course.Id,
            IsPublished = course.IsPublished
        }, Guid.NewGuid().ToString());

        return Result<CourseReadDto>.Success(_mapper.Map<CourseReadDto>(course));
    }

    public async Task<Result<LessonReadDto>> AddLessonAsync(LessonCreateDto lessonCreateDto)
    {
        if (await _repository.CourseExistsAsync(lessonCreateDto.CourseId) == false)
        {
            return Result<LessonReadDto>.Failure(CourseErrors.CourseNotFound);
        }

        var lesson = await CreateLesson(lessonCreateDto);
        await _repository.AddLessonAsync(lesson);
        await _repository.SaveChangesAsync();

        return Result<LessonReadDto>.Success(_mapper.Map<LessonReadDto>(lesson));
    }

    public async Task<Result<CourseReadDto>> DeleteCourseAsync(int courseId)
    {
        var course = await _repository.GetCourseByIdAsync(courseId, false);

        if (course is null)
        {
            return Result<CourseReadDto>.Failure(CourseErrors.CourseNotFound);
        }

        _repository.DeleteCourse(course);
        await _repository.SaveChangesAsync();
        await _publisher.PublishCourseRemovedEvent(course.Id, Guid.NewGuid().ToString());

        return Result<CourseReadDto>.Success(_mapper.Map<CourseReadDto>(course));
    }

    public async Task<Result<LessonReadDto>> DeleteLessonAsync(int lessonId)
    {
        var lesson = await _repository.GetLessonByIdAsync(lessonId);

        if (lesson is null)
        {
            return Result<LessonReadDto>.Failure(CourseErrors.LessonNotFound);
        }

        _repository.DeleteLesson(lesson);
        await _repository.SaveChangesAsync();

        return Result<LessonReadDto>.Success(_mapper.Map<LessonReadDto>(lesson));
    }

    public async Task<Result<CourseReadDto>> GetCourseByIdAsync(int courseId, bool includeLessons = false)
    {
        var course = await _repository.GetCourseByIdAsync(courseId, includeLessons);

        if (course is null)
            return Result<CourseReadDto>.Failure(CourseErrors.CourseNotFound);

        return Result<CourseReadDto>.Success(_mapper.Map<CourseReadDto>(course));
    }

    public async Task<Result<LessonReadDto>> GetLessonByIdAsync(int lessonId)
    {
        var lesson = await _repository.GetLessonByIdAsync(lessonId);

        if (lesson is null)
        {
            return Result<LessonReadDto>.Failure(CourseErrors.LessonNotFound);
        }

        return Result<LessonReadDto>.Success(_mapper.Map<LessonReadDto>(lesson));
    }

    public async Task<PagedResponseDto<LessonReadDto>> GetLessonsAsync(LessonSearchDto query)
    {
        var (lessons, totalCount) = await _repository.GetLessonsAsync(query);

        return new PagedResponseDto<LessonReadDto>
        {
            Items = _mapper.Map<List<LessonReadDto>>(lessons),
            TotalCount = totalCount,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber
        };
    }

    public async Task<PagedResponseDto<CourseReadDto>> SearchCoursesAsync(CourseSearchDto query)
    {
        var (courses, totalCount) = await _repository.SearchCoursesAsync(query);

        return new PagedResponseDto<CourseReadDto>
        {
            Items = _mapper.Map<List<CourseReadDto>>(courses),
            TotalCount = totalCount,
            PageSize = query.PageSize ?? 10,
            PageNumber = query.PageNumber ?? 1
        };
    }

    private Course CreateCourse(CourseCreateDto courseCreateDto)
    {
        var course = _mapper.Map<Course>(courseCreateDto);
        course.IsPublished = false;
        course.Lessons = new List<Lesson>();

        return course;
    }

    private async Task<Lesson> CreateLesson(LessonCreateDto lessonCreateDto)
    {
        var lesson = _mapper.Map<Lesson>(lessonCreateDto);
        lesson.IsPreview = false;
        lesson.Order = await _repository.GetLastLessonOrder(lessonCreateDto.CourseId) + 1;

        return lesson;
    }
}
