import { useQuery } from "@tanstack/react-query";
import { fetchLessonById } from "@/features/lessons/api";
import type { Lesson } from "../types";

export function useLesson(id: string | undefined) {
  return useQuery<Lesson, Error>({
    queryKey: ["lessons", id],
    queryFn: () => fetchLessonById(id!),
    enabled: !!id,
  });
}
