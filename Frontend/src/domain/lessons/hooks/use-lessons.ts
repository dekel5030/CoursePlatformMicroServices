import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchLessonById,
  createLesson,
  patchLesson,
  deleteLesson,
  generateLessonAi,
  moveLesson,
} from "../api";
import type { MoveLessonRequest } from "../api";
import type {
  LessonModel,
  CreateLessonRequestDto,
  UpdateLessonRequestDto,
} from "../types";
import { lessonsQueryKeys } from "../query-keys";
import { coursesQueryKeys } from "@/domain/courses/query-keys";
import { toast } from "sonner";

export function useLesson(
  courseId: string,
  lessonId: string | undefined,
  url?: string,
) {
  return useQuery<LessonModel, Error>({
    queryKey: lessonId
      ? lessonsQueryKeys.detail(courseId, lessonId)
      : ["lessons", courseId, "undefined"],
    queryFn: () => fetchLessonById(courseId, lessonId!, url),
    enabled: !!courseId && !!lessonId,
  });
}

export function useCreateLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: CreateLessonRequestDto;
    }) => createLesson(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.all(courseId),
      });
      // Note: Course cache invalidation should be handled by the feature layer
    },
  });
}

export function usePatchLesson(courseId: string, lessonId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: UpdateLessonRequestDto;
    }) => patchLesson(url, request),
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
        queryKey: lessonsQueryKeys.all(courseId),
      });
      // Note: Course cache invalidation should be handled by the feature layer
      toast.success("Lesson deleted successfully");
    },
    onError: (error) => {
      console.error("Failed to delete lesson:", error);
      toast.error("Failed to delete lesson");
    },
  });
}

export function useGenerateLessonAi() {
  return useMutation({
    mutationFn: (url: string) => generateLessonAi(url),
  });
}

export function useMoveLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      lessonId,
      request,
    }: {
      lessonId: string;
      request: MoveLessonRequest;
    }) => moveLesson(lessonId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: lessonsQueryKeys.all(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      toast.success("Lesson moved successfully");
    },
    onError: () => {
      toast.error("Failed to move lesson");
    },
  });
}
