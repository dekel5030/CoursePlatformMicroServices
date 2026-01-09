import { axiosClient } from "@/axios";
import type { Lesson } from "../types";

export async function fetchLessonById(
  courseId: string,
  lessonId: string
): Promise<Lesson> {
  const response = await axiosClient.get<Lesson>(
    `/courses/${courseId}/lessons/${lessonId}`
  );
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

export interface PatchLessonRequest {
  title?: string;
  description?: string;
  access?: string;
}

export async function patchLesson(
  courseId: string,
  lessonId: string,
  request: PatchLessonRequest
): Promise<void> {
  await axiosClient.patch(`/courses/${courseId}/lessons/${lessonId}`, request);
}

export async function deleteLesson(
  courseId: string,
  lessonId: string
): Promise<void> {
  await axiosClient.delete(`/courses/${courseId}/lessons/${lessonId}`);
}
