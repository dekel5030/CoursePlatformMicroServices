using AutoMapper;
using Common;
using Common.Errors;
using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using CourseService.Data.CoursesRepo;
using CourseService.Data.UnitOfWork;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _producer = "CourseService";

    public CourseService(
        ICourseRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ICourseEventPublisher publisher)
    {
        _repository = repository;
        _mapper = mapper;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseReadDto>> AddCourseAsync(CourseCreateDto courseCreateDto)
    {
        var course = CreateCourse(courseCreateDto);

        await _repository.AddCourseAsync(course);
        await _unitOfWork.SaveChangesAsync();
        await PublishCourseUpsertedEvent(course, true);

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
        await _unitOfWork.SaveChangesAsync();

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
        await _unitOfWork.SaveChangesAsync();
        await PublishCourseUpsertedEvent(course, false);

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
        await _unitOfWork.SaveChangesAsync();

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

    private Task PublishCourseUpsertedEvent(
        Course course, bool isPublished, string? correlationId = null,
        string? eventId = null, CancellationToken ct = default)
    {
        CourseUpsertedV1 payload = new CourseUpsertedV1
        (
            CourseId: course.Id,
            IsPublished: isPublished
        );

        EventEnvelope<CourseUpsertedV1> envelope =
            EventEnvelope<CourseUpsertedV1>
                .Create(producer: _producer,
                        aggregateId: course.Id.ToString(),
                        aggregateVersion: course.AggregateVersion,
                        payload: payload,
                        contractVersion: CourseUpsertedV1.Version.ToString(),
                        correlationId: correlationId,
                        eventId: eventId
                );

        return _publisher.PublishCourseUpsertedEvent(envelope, ct: ct);
    }
}
