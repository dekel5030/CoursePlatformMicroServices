import { axiosClient } from "@/app/axios";
import type { CurrentUserDto } from "../types/CurrentUserDto";
import type { UserModel } from "../types/UserModel";
import { mapToUserModel } from "../mappers";

/**
 * Fetch the current authenticated user
 */
export async function fetchCurrentUser(): Promise<UserModel> {
  const response = await axiosClient.get<CurrentUserDto>("auth/me");
  return mapToUserModel(response.data);
}
