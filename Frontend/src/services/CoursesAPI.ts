import type { Course } from "../types/course";
import type { Lesson } from "../types/Lesson";
import { apiClient } from '../lib/apiClient';

export async function fetchFeaturedCourses(token?: string): Promise<Course[]> {
  const data = await apiClient.get<Course[] | { items: Course[] }>('/courses/featured', { token });
  return Array.isArray(data) ? data : data.items || [];
}

export async function fetchCourseById(id: string, token?: string): Promise<Course> {
  return apiClient.get<Course>(`/courses/${id}`, { token });
}

export async function fetchLessonById(id: string, token?: string): Promise<Lesson> {
  return apiClient.get<Lesson>(`/lessons/${id}`, { token });
}
