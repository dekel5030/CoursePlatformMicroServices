import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchLessonById, createLesson, patchLesson, deleteLesson } from "../api";
import type { Lesson } from "../types";
import type { CreateLessonRequest, PatchLessonRequest } from "../api";
import { coursesQueryKeys } from "@/features/courses/hooks/use-courses";

// Centralized Query Keys
export const lessonsQueryKeys = {
  all: ["lessons"] as const,
  detail: (id: string) => [...lessonsQueryKeys.all, id] as const,
} as const;

// Lesson Queries
export function useLesson(id: string | undefined) {
  return useQuery<Lesson, Error>({
    queryKey: id ? lessonsQueryKeys.detail(id) : ["lessons", "undefined"],
    queryFn: () => fetchLessonById(id!),
    enabled: !!id,
  });
}

// Lesson Mutations
export function useCreateLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateLessonRequest) => createLesson(courseId, request),
    onSuccess: () => {
      // Invalidate the course detail to update the lessons list
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
    },
  });
}

export function usePatchLesson(id: string, courseId?: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: PatchLessonRequest) => patchLesson(id, request),
    onSuccess: () => {
      // Invalidate the specific lesson
      queryClient.invalidateQueries({ queryKey: lessonsQueryKeys.detail(id) });
      // Invalidate the course detail if courseId is provided (to update lessons list)
      if (courseId) {
        queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
      }
    },
  });
}

export function useDeleteLesson(courseId?: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteLesson(id),
    onSuccess: () => {
      // Invalidate all lessons
      queryClient.invalidateQueries({ queryKey: lessonsQueryKeys.all });
      // Invalidate the course detail if courseId is provided (to update lessons list)
      if (courseId) {
        queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
      }
    },
  });
}
