import { apiClient } from "@/api";

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
  return apiClient.get<User>(`/users/${id}`);
}

export async function updateUser(
  id: string,
  data: UpdateUserRequest
): Promise<User> {
  return apiClient.put<User>(`/users/${id}`, data);
}
