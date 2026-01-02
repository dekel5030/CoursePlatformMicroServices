import type { Course } from "../types";
import { axiosClient } from "@/axios/axiosClient";

export async function fetchFeaturedCourses(): Promise<Course[]> {
  const response = await axiosClient.get<Course[] | { items: Course[] }>(
    "/courses/featured"
  );
  const data = response.data;

  return Array.isArray(data) ? data : data.items || [];
}

export async function fetchCourseById(id: string): Promise<Course> {
  const response = await axiosClient.get<Course>(`/courses/${id}`);
  return response.data;
}
