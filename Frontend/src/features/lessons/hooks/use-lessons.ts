import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchLessonById, createLesson } from "../api";
import type { Lesson } from "../types";
import type { CreateLessonRequest } from "../api";

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
