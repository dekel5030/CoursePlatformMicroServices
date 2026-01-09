export const CourseRoutes = {
  catalog: () => "/catalog",
  allCourses: () => "/courses",
  courseDetail: (courseId: string) => `/courses/${courseId}`,
} as const;

export const CourseResources = {
  RESOURCE_TYPE: "Course" as const,
} as const;
