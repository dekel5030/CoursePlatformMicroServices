/**
 * Centralized constants for course routes and resource identifiers
 * Ensures consistency across components, hooks, and routing
 */

/**
 * Route path builders for courses
 */
export const CourseRoutes = {
  /**
   * Get the path to the course catalog
   */
  catalog: () => "/catalog",

  /**
   * Get the path to all courses
   */
  allCourses: () => "/courses",

  /**
   * Get the path to a specific course page
   */
  courseDetail: (courseId: string) => `/courses/${courseId}`,
} as const;

/**
 * Resource identifiers for permission checks
 */
export const CourseResources = {
  RESOURCE_TYPE: "Course" as const,
} as const;
