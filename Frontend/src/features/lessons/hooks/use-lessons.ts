import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchLessonById, createLesson, patchLesson } from "../api";
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
      queryClient.invalidateQueries({ queryKey: ["courses", courseId] });
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
      // Also invalidate the course detail if courseId is provided (to update lessons list)
      if (courseId) {
        queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
      }
    },
  });
}
