import { axiosClient } from "@/api/axiosClient";

/**
 * Users API - Stateless service layer using Axios
 * No React hooks allowed in this file
 */

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

/**
 * Fetch user by ID
 */
export async function fetchUserById(id: string): Promise<User> {
  const response = await axiosClient.get<User>(`/users/${id}`);
  return response.data;
}

/**
 * Update user profile
 */
export async function updateUser(
  id: string,
  data: UpdateUserRequest
): Promise<User> {
  const response = await axiosClient.put<User>(`/users/${id}`, data);
  return response.data;
}
