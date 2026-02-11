import { useQuery } from "@tanstack/react-query";
import { fetchCourseAnalytics } from "../api/courses-api";
import type { CourseDetailedAnalyticsDto } from "../types";
import { coursesQueryKeys } from "../query-keys";

/**
 * Hook to fetch detailed analytics for a course (instructor only)
 */
export function useCourseAnalytics(courseId: string | undefined) {
  return useQuery<CourseDetailedAnalyticsDto, Error>({
    queryKey: courseId
      ? coursesQueryKeys.analytics(courseId)
      : ["courses", "analytics", "undefined"],
    queryFn: () => fetchCourseAnalytics(courseId!),
    enabled: !!courseId,
  });
}
