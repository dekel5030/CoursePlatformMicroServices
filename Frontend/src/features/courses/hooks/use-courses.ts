import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchFeaturedCourses, fetchCourseById, createCourse, patchCourse, deleteCourse, fetchAllCourses } from "../api";
import type { CourseModel, CreateCourseRequestDto, UpdateCourseRequestDto } from "../types";

// Centralized Query Keys
export const coursesQueryKeys = {
  all: ["courses"] as const,
  featured: () => [...coursesQueryKeys.all, "featured"] as const,
  allCourses: () => [...coursesQueryKeys.all, "list"] as const,
  detail: (id: string) => [...coursesQueryKeys.all, id] as const,
} as const;

// Course Queries
export function useFeaturedCourses() {
  return useQuery<CourseModel[], Error>({
    queryKey: coursesQueryKeys.featured(),
    queryFn: fetchFeaturedCourses,
  });
}

export function useAllCourses() {
  return useQuery<CourseModel[], Error>({
    queryKey: coursesQueryKeys.allCourses(),
    queryFn: fetchAllCourses,
  });
}

export function useCourse(id: string | undefined) {
  return useQuery<CourseModel, Error>({
    queryKey: id ? coursesQueryKeys.detail(id) : ["courses", "undefined"],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}

// Course Mutations
export function useCreateCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCourseRequestDto) => createCourse(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}

export function usePatchCourse(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateCourseRequestDto) => patchCourse(id, request),
    onSuccess: () => {
      // Invalidate the specific course and featured courses
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.featured() });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.allCourses() });
    },
  });
}

export function useDeleteCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteCourse(id),
    onSuccess: () => {
      // Invalidate all course lists to ensure UI updates
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}
