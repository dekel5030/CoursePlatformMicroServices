using Courses.Application.Services.LinkProvider.Abstractions.Links;

namespace Courses.Application.Services.LinkProvider.Abstractions;

/// <summary>
/// URL-only registry for building links. No authorization or business logic.
/// </summary>
public interface ILinkProvider
{
    // Course
    LinkRecord GetManagedCourseLink(Guid courseId);
    LinkRecord GetManagedCoursesLink(int pageNumber, int pageSize);
    LinkRecord GetCoursePageLink(Guid courseId);
    LinkRecord GetCourseAnalyticsLink(Guid courseId);
    LinkRecord GetPatchCourseLink(Guid courseId);
    LinkRecord GetDeleteCourseLink(Guid courseId);
    LinkRecord GetPublishCourseLink(Guid courseId);
    LinkRecord GetGenerateCourseImageUploadUrlLink(Guid courseId);
    LinkRecord GetCreateModuleLink(Guid courseId);
    LinkRecord GetReorderModulesLink(Guid courseId);
    LinkRecord GetCourseRatingsLink(Guid courseId);
    LinkRecord GetCoursesLink(int pageNumber, int pageSize);

    // Module
    LinkRecord GetCreateLessonLink(Guid moduleId);
    LinkRecord GetPatchModuleLink(Guid moduleId);
    LinkRecord GetDeleteModuleLink(Guid moduleId);
    LinkRecord GetReorderLessonsLink(Guid moduleId);

    // Lesson
    LinkRecord GetLessonPageLink(Guid lessonId);
    LinkRecord GetPatchLessonLink(Guid lessonId);
    LinkRecord GetLessonVideoUploadUrlLink(Guid lessonId);
    LinkRecord GetGenerateLessonWithAiLink(Guid lessonId);
    LinkRecord GetMoveLessonLink(Guid lessonId);

    // Enrolled courses (current user)
    LinkRecord GetEnrolledCoursesLink(int pageNumber, int pageSize);

    // Course ratings (create/update/delete)
    LinkRecord GetCreateCourseRatingLink(Guid courseId);
    LinkRecord GetUpdateCourseRatingLink(Guid ratingId);
    LinkRecord GetDeleteCourseRatingLink(Guid ratingId);
}
