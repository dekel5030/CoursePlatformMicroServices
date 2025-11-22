import type { Course } from "../types/course";
import type { Lesson } from "../types/Lesson";

const API_COURSES_URL = "/courseservice/api";

export async function fetchFeaturedCourses(): Promise<Course[]> {
  const response = await fetch(`${API_COURSES_URL}/courses/featured`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch featured courses");

  const data = await response.json();
  return data.items;
}

export async function fetchCourseById(id: string): Promise<Course> {
  const response = await fetch(`${API_COURSES_URL}/courses/${id}`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch course");

  return await response.json();
}

export async function fetchLessonById(id: string): Promise<Lesson> {
  const response = await fetch(`${API_COURSES_URL}/lessons/${id}`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch lesson");

  return await response.json();
}
