import type { Course, Lesson } from "@/types";
import { axiosClient } from "@/api/axiosClient";

/**
 * Courses API - Stateless service layer using Axios
 * No React hooks allowed in this file
 */

/**
 * Fetch featured courses
 */
export async function fetchFeaturedCourses(): Promise<Course[]> {
  const response = await axiosClient.get<Course[] | { items: Course[] }>('/courses/featured');
  const data = response.data;
  
  // Handle both array and object with items property
  return Array.isArray(data) ? data : data.items || [];
}

/**
 * Fetch a single course by ID
 */
export async function fetchCourseById(id: string): Promise<Course> {
  const response = await axiosClient.get<Course>(`/courses/${id}`);
  return response.data;
}

/**
 * Fetch a single lesson by ID
 */
export async function fetchLessonById(id: string): Promise<Lesson> {
  const response = await axiosClient.get<Lesson>(`/lessons/${id}`);
  return response.data;
}
