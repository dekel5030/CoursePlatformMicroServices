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
    LinkRecord GetCreateCourseLink();
    LinkRecord GetCoursePageLink(Guid courseId);
    LinkRecord GetCourseAnalyticsLink(Guid courseId);
    LinkRecord GetPatchCourseLink(Guid courseId);
    LinkRecord GetDeleteCourseLink(Guid courseId);
    LinkRecord GetPublishCourseLink(Guid courseId);
    LinkRecord GetGenerateCourseImageUploadUrlLink(Guid courseId);
    LinkRecord GetCreateModuleLink(Guid courseId);
    LinkRecord GetChangePositionForCourse(Guid courseId);
    LinkRecord GetCourseRatingsLink(Guid courseId);
    LinkRecord GetCourseRatingsLink(Guid courseId, int pageNumber, int pageSize);
    LinkRecord GetCoursesLink(int pageNumber, int pageSize);

    // Module
    LinkRecord GetCreateLessonLink(Guid moduleId);
    LinkRecord GetPatchModuleLink(Guid moduleId);
    LinkRecord GetDeleteModuleLink(Guid moduleId);
    LinkRecord GetChangePositionForModule(Guid moduleId);

    // Lesson
    LinkRecord GetLessonPageLink(Guid lessonId);
    LinkRecord GetManagedLessonPageLink(Guid lessonId);
    LinkRecord GetPatchLessonLink(Guid lessonId);
    LinkRecord GetDeleteLessonLink(Guid lessonId);
    LinkRecord GetLessonVideoUploadUrlLink(Guid lessonId);
    LinkRecord GetGenerateLessonWithAiLink(Guid lessonId);
    LinkRecord GetChangePositionForLesson(Guid lessonId);
    LinkRecord GetLessonTranscriptLink(Guid lessonId);

    /// <summary>
    /// Returns a link with placeholder href for unimplemented endpoints. Use when a DTO requires a link but the backend is not ready.
    /// </summary>
    LinkRecord GetPlaceholderLink(string method = "GET");

    // Enrolled courses (current user)
    LinkRecord GetEnrolledCoursesLink(int pageNumber, int pageSize);

    // Enrollment progress and completion
    LinkRecord GetUpdateEnrollmentProgressLink(Guid enrollmentId);
    LinkRecord GetMarkEnrollmentLessonCompletedLink(Guid enrollmentId, Guid lessonId);

    // Course ratings (create/update/delete)
    LinkRecord GetCreateCourseRatingLink(Guid courseId);
    LinkRecord GetUpdateCourseRatingLink(Guid ratingId);
    LinkRecord GetDeleteCourseRatingLink(Guid ratingId);
}
