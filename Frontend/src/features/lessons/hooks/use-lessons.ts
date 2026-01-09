import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchLessonById,
  createLesson,
  patchLesson,
  deleteLesson,
} from "../api";
import type { LessonModel, CreateLessonRequestDto, UpdateLessonRequestDto } from "../types";
import { coursesQueryKeys } from "@/features/courses/hooks/use-courses";

export const lessonsQueryKeys = {
  all: (courseId: string) =>
    [...coursesQueryKeys.detail(courseId), "lessons"] as const,
  detail: (courseId: string, lessonId: string) =>
    [...lessonsQueryKeys.all(courseId), lessonId] as const,
} as const;

export function useLesson(courseId: string, lessonId: string | undefined) {
  return useQuery<LessonModel, Error>({
    queryKey: lessonId
      ? lessonsQueryKeys.detail(courseId, lessonId)
      : ["courses", courseId, "lessons", "undefined"],
    queryFn: () => fetchLessonById(courseId, lessonId!),
    enabled: !!courseId && !!lessonId,
  });
}

export function useCreateLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateLessonRequestDto) =>
      createLesson(courseId, request),
    onSuccess: () => {
      // מעדכן את רשימת השיעורים בתוך הקורס
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.all(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
    },
  });
}

export function usePatchLesson(courseId: string, lessonId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateLessonRequestDto) =>
      patchLesson(courseId, lessonId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.detail(courseId, lessonId),
      });
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.all(courseId),
      });
    },
  });
}

export function useDeleteLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (lessonId: string) => deleteLesson(courseId, lessonId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.all(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
    },
  });
}
