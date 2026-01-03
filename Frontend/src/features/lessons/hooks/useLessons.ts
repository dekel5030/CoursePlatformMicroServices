import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createLesson, type CreateLessonRequest } from "@/features/lessons/api";

export function useCreateLesson(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateLessonRequest) => createLesson(courseId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["courses", courseId] });
    },
  });
}
