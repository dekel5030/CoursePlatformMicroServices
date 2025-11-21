export const API_COURSES_URL =
  import.meta.env.VITE_API_COURSES_URL || "https://localhost:7171/api";

export const API_USERS_URL =
  import.meta.env.VITE_API_USERS_URL || "https://localhost:7079/api";

export const API_AUTH_URL =
  import.meta.env.VITE_API_AUTH_URL || "https://localhost:7233/auth";

export async function fetchFeaturedCourses() {
  const response = await fetch(`${API_COURSES_URL}/courses/featured`);
  if (!response.ok) throw new Error("Failed to fetch featured courses");

  const data = await response.json();
  return data.items;
}

export async function fetchCourseById(id: string) {
  const response = await fetch(`${API_COURSES_URL}/courses/${id}`);
  if (!response.ok) throw new Error("Failed to fetch course");

  return await response.json();
}

export async function fetchLessonById(id: string) {
  const response = await fetch(`${API_COURSES_URL}/lessons/${id}`);
  if (!response.ok) throw new Error("Failed to fetch lesson");

  return await response.json();
}

export async function fetchUserById(id: string) {
  const response = await fetch(`${API_USERS_URL}/users/${id}`);
  if (!response.ok) throw new Error("Failed to fetch user");

  return await response.json();
}

export async function loginUser(data: { email: string; password: string }) {
  const response = await fetch(`${API_AUTH_URL}/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Login failed");
  }

  return response.json();
}
