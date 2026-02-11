/**
 * React Query key factory for courses
 * Centralized query keys to avoid duplication and ensure consistency
 */
export const coursesQueryKeys = {
  all: ["courses"] as const,
  featured: () => [...coursesQueryKeys.all, "featured"] as const,
  allCourses: () => [...coursesQueryKeys.all, "list"] as const,
  detail: (id: string) => [...coursesQueryKeys.all, id] as const,
  managedDetail: (id: string) =>
    [...coursesQueryKeys.all, "managed", id] as const,
  ratings: (courseId: string, pageKey?: string | number, pageSize?: number) =>
    [...coursesQueryKeys.all, courseId, "ratings", pageKey, pageSize] as const,
  managed: (pageNumber: number, pageSize: number) =>
    [...coursesQueryKeys.all, "managed", pageNumber, pageSize] as const,
} as const;
