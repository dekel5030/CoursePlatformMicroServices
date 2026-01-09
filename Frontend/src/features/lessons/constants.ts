export const LessonRoutes = {
  lessonDetail: (courseId: string, lessonId: string) =>
    `/courses/${courseId}/lessons/${lessonId}`,
} as const;

export const LessonResources = {
  RESOURCE_TYPE: "Lesson" as const,
} as const;
