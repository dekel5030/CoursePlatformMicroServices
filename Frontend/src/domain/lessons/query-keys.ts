/**
 * React Query key factory for lessons
 * Centralized query keys - independent of courses domain
 */
export const lessonsQueryKeys = {
  all: (courseId: string) => ["lessons", courseId] as const,
  detail: (courseId: string, lessonId: string, url?: string) =>
    [...lessonsQueryKeys.all(courseId), lessonId, url ?? "default"] as const,
} as const;
