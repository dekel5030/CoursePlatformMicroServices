import { getAuthenticatedFetch } from "../utils/authenticatedFetch";

const API_USERS_URL = "api/userservice/api";

export interface User {
  id: string;
  email: string;
  firstName?: string | null;
  lastName?: string | null;
  dateOfBirth?: string | null;
  phoneNumber?: string | null;
}

export interface UpdateUserRequest {
  firstName?: string | null;
  lastName?: string | null;
  phoneNumber?: { countryCode: string; number: string } | null;
  dateOfBirth?: string | null;
}

export async function fetchUserById(id: string): Promise<User> {
  const authFetch = getAuthenticatedFetch();
  const response = await authFetch(`${API_USERS_URL}/users/${id}`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch user");

  return await response.json();
}

export async function updateUser(
  id: string,
  data: UpdateUserRequest
): Promise<User> {
  const authFetch = getAuthenticatedFetch();
  const response = await authFetch(`${API_USERS_URL}/users/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!response.ok) throw new Error("Failed to update user");

  return await response.json();
}
