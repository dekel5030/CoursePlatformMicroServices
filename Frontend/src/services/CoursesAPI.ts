import type { Course } from "../types/course";
import type { Lesson } from "../types/Lesson";

const API_COURSES_URL = "/api";

export type Fetcher = (
  input: RequestInfo | URL,
  init?: RequestInit
) => Promise<Response>;

export async function fetchFeaturedCourses(
  fetcher: Fetcher
): Promise<Course[]> {
  const response = await fetcher(`${API_COURSES_URL}/courses/featured`);
  if (!response.ok) throw new Error("Failed to fetch featured courses");
  const data = await response.json();
  return data.items || data;
}

export async function fetchCourseById(
  id: string,
  fetcher: Fetcher
): Promise<Course> {
  const response = await fetcher(`${API_COURSES_URL}/courses/${id}`);
  if (!response.ok) throw new Error("Failed to fetch course");
  return await response.json();
}

export async function fetchLessonById(
  id: string,
  fetcher: Fetcher
): Promise<Lesson> {
  const response = await fetcher(`${API_COURSES_URL}/lessons/${id}`);
  if (!response.ok) throw new Error("Failed to fetch lesson");
  return await response.json();
}
