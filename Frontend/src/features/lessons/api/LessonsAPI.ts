import { axiosClient } from "@/axios";
import type { Lesson } from "../types";

export async function fetchLessonById(id: string): Promise<Lesson> {
  const response = await axiosClient.get<Lesson>(`/lessons/${id}`);
  return response.data;
}
