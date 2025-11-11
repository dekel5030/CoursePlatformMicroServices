export const API_COURSES_URL =
  import.meta.env.VITE_API_COURSES_URL || "https://localhost:7171/api";

export const API_USERS_URL =
  import.meta.env.VITE_API_USERS_URL || "http://localhost:234";

export async function fetchFeaturedCourses() {
  const response = await fetch(`${API_COURSES_URL}/courses/featured`);
  if (!response.ok) throw new Error("Failed to fetch featured courses");

  const data = await response.json();
  return data.items;
}
