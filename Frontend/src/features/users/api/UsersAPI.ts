import { axiosClient } from "@/axios/axiosClient";
import type { User } from "../types/User";
import type { UpdateUserRequest } from "../types/UpdateUserRequest";

export async function fetchUserById(id: string): Promise<User> {
  const response = await axiosClient.get<User>(`/users/${id}`);
  return response.data;
}

export async function updateUser(
  id: string,
  data: UpdateUserRequest
): Promise<User> {
  const response = await axiosClient.put<User>(`/users/${id}`, data);
  return response.data;
}
