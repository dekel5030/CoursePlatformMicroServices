import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchFeaturedCourses, fetchCourseById, createCourse, patchCourse } from "../api";
import type { Course } from "../types";
import type { CreateCourseRequest, PatchCourseRequest } from "../api";

// Centralized Query Keys
export const coursesQueryKeys = {
  all: ["courses"] as const,
  featured: () => [...coursesQueryKeys.all, "featured"] as const,
  detail: (id: string) => [...coursesQueryKeys.all, id] as const,
} as const;

// Course Queries
export function useFeaturedCourses() {
  return useQuery<Course[], Error>({
    queryKey: coursesQueryKeys.featured(),
    queryFn: fetchFeaturedCourses,
  });
}

export function useCourse(id: string | undefined) {
  return useQuery<Course, Error>({
    queryKey: id ? coursesQueryKeys.detail(id) : ["courses", "undefined"],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}

// Course Mutations
export function useCreateCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCourseRequest) => createCourse(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}

export function usePatchCourse(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: PatchCourseRequest) => patchCourse(id, request),
    onSuccess: () => {
      // Invalidate the specific course and featured courses
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.featured() });
    },
  });
}
