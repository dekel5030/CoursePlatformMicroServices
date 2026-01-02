import { axiosClient } from "@/axios/axiosClient";
import type { CurrentUserDto } from "../types/CurrentUserDto";

export async function fetchCurrentUser(): Promise<CurrentUserDto> {
  const response = await axiosClient.get<CurrentUserDto>("auth/me");
  return response.data;
}
