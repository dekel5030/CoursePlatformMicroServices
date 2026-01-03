import { axiosClient } from "@/axios";
import type { Lesson } from "../types";

export async function fetchLessonById(id: string): Promise<Lesson> {
  const response = await axiosClient.get<Lesson>(`/lessons/${id}`);
  return response.data;
}

export interface CreateLessonRequest {
  title: string;
  description?: string;
}

export async function createLesson(
  courseId: string,
  request: CreateLessonRequest
): Promise<Lesson> {
  const response = await axiosClient.post<Lesson>(
    `/courses/${courseId}/lessons`,
    request
  );
  return response.data;
}
