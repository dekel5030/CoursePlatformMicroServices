/**
 * Backend DTO: Matches CourseDetailedAnalyticsDto from CourseService
 * Detailed analytics for instructor's course analytics view (GET /manage/courses/{id}/analytics)
 */
export interface CourseDetailedAnalyticsDto {
  enrollmentsCount: number;
  averageRating: number;
  reviewsCount: number;
  viewCount: number;
  totalLessonsCount: number;
  totalCourseDuration: string;
  moduleAnalytics: ModuleAnalyticsSummaryDto[];
  enrollmentsOverTime: EnrollmentCountByDayDto[];
  courseViewers: CourseViewerDto[];
}

/**
 * A user who viewed the course (for analytics viewer list).
 */
export interface CourseViewerDto {
  userId: string;
  displayName: string;
  avatarUrl: string | null;
  viewedAt: string;
}

/**
 * Per-module analytics summary
 */
export interface ModuleAnalyticsSummaryDto {
  moduleId: string;
  lessonCount: number;
  totalDuration: string;
}

/**
 * Time-series enrollment data
 */
export interface EnrollmentCountByDayDto {
  date: string;
  count: number;
}
