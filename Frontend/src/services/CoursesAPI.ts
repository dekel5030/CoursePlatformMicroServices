import type { Course, Lesson } from "@/types";
import { axiosClient } from "@/api/axiosClient";

export async function fetchFeaturedCourses(): Promise<Course[]> {
  const response = await axiosClient.get<Course[] | { items: Course[] }>('/courses/featured');
  const data = response.data;
  
  return Array.isArray(data) ? data : data.items || [];
}

export async function fetchCourseById(id: string): Promise<Course> {
  const response = await axiosClient.get<Course>(`/courses/${id}`);
  return response.data;
}

export async function fetchLessonById(id: string): Promise<Lesson> {
  const response = await axiosClient.get<Lesson>(`/lessons/${id}`);
  return response.data;
}
