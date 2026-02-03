import type { EnrolledCourseDto } from "../types/EnrolledCourseDto";
import type { EnrolledCourseWithAnalyticsDto } from "../types/EnrolledCourseWithAnalyticsDto";

/**
 * Maps EnrolledCourseWithAnalyticsDto to flat EnrolledCourseDto for components
 */
export function mapEnrolledCourseWithAnalyticsToDto(
  dto: EnrolledCourseWithAnalyticsDto,
): EnrolledCourseDto {
  const { enrolledCourse, analytics } = dto;
  return {
    ...enrolledCourse,
    progressPercentage: analytics.progressPercentage,
  };
}
