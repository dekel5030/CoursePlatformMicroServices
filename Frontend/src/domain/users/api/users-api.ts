import { axiosClient } from "@/app/axios";
import type { User, UpdateUserRequest } from "../types";

/**
 * Fetch a user by ID
 */
export async function fetchUserById(id: string): Promise<User> {
  const response = await axiosClient.get<User>(`/users/${id}`);
  return response.data;
}

/**
 * Update a user
 */
export async function updateUser(
  id: string,
  data: UpdateUserRequest
): Promise<User> {
  const response = await axiosClient.put<User>(`/users/${id}`, data);
  return response.data;
}
