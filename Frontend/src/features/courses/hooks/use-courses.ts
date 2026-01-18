import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchFeaturedCourses,
  fetchCourseById,
  createCourse,
  patchCourse,
  deleteCourse,
  fetchAllCourses,
  type FetchAllCoursesResult,
} from "../api";
import type {
  CourseModel,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
} from "../types";
import { API_ENDPOINTS } from "@/axios/config";

export const coursesQueryKeys = {
  all: ["courses"] as const,
  featured: () => [...coursesQueryKeys.all, "featured"] as const,
  allCourses: () => [...coursesQueryKeys.all, "list"] as const,
  detail: (id: string) => [...coursesQueryKeys.all, id] as const,
} as const;

export function useFeaturedCourses() {
  return useQuery<CourseModel[], Error>({
    queryKey: coursesQueryKeys.featured(),
    queryFn: fetchFeaturedCourses,
  });
}

export function useAllCourses(url?: string) {
  return useQuery<FetchAllCoursesResult, Error>({
    queryKey: [...coursesQueryKeys.allCourses(), url || API_ENDPOINTS.COURSES],
    queryFn: () => fetchAllCourses(url),
  });
}

export function useCourse(id: string | undefined) {
  return useQuery<CourseModel, Error>({
    queryKey: id ? coursesQueryKeys.detail(id) : ["courses", "undefined"],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}

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
    mutationFn: ({ url, request }: { url: string; request: UpdateCourseRequestDto }) => 
      patchCourse(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.featured() });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.allCourses(),
      });
    },
  });
}

export function useDeleteCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (url: string) => deleteCourse(url),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}
