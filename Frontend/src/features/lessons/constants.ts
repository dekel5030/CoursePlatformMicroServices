/**
 * Centralized constants for lesson routes and resource identifiers
 * Ensures consistency across components, hooks, and routing
 */

/**
 * Route path builders for lessons
 */
export const LessonRoutes = {
  /**
   * Get the path to a specific lesson page
   */
  lessonDetail: (courseId: string, lessonId: string) =>
    `/courses/${courseId}/lessons/${lessonId}`,
} as const;

/**
 * Resource identifiers for permission checks
 */
export const LessonResources = {
  RESOURCE_TYPE: "Lesson" as const,
} as const;
