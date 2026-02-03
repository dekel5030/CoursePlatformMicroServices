import type { EnrolledCourseDtoApi } from "./EnrolledCourseDto";
import type { EnrolledCourseAnalyticsDto } from "./EnrolledCourseAnalyticsDto";

/**
 * Backend DTO: Matches EnrolledCourseWithAnalyticsDto from CourseService
 */
export interface EnrolledCourseWithAnalyticsDto {
  enrolledCourse: EnrolledCourseDtoApi;
  analytics: EnrolledCourseAnalyticsDto;
}
