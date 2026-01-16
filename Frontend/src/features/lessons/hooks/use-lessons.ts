import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchLessonById,
  createLesson,
  patchLesson,
  deleteLesson,
} from "../api";
import type {
  LessonModel,
  CreateLessonRequestDto,
  UpdateLessonRequestDto,
} from "../types";
import { coursesQueryKeys } from "@/features/courses/hooks/use-courses";
import { toast } from "sonner";

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
    mutationFn: ({ url, request }: { url: string; request: CreateLessonRequestDto }) =>
      createLesson(url, request),
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

export function usePatchLesson(courseId: string, lessonId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ url, request }: { url: string; request: UpdateLessonRequestDto }) =>
      patchLesson(url, request),
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
    mutationFn: (deleteUrl: string) => deleteLesson(deleteUrl),

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      toast.success("Lesson deleted successfully");
    },
    onError: (error) => {
      console.error("Failed to delete lesson:", error);
      toast.error("Failed to delete lesson");
    },
  });
}
