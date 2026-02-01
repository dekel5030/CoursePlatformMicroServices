import { axiosClient } from "@/app/axios";
import type { CurrentUserDto } from "../types/CurrentUserDto";
import type { UserModel } from "../types/UserModel";

export function mapToUserModel(dto: CurrentUserDto): UserModel {
  return {
    id: dto.id,
    email: dto.email,
    firstName: dto.firstName,
    lastName: dto.lastName,
    fullName: `${dto.firstName} ${dto.lastName}`,
    roles: dto.roles,
    permissions: dto.permissions,
  };
}

export async function fetchCurrentUser(): Promise<UserModel> {
  const response = await axiosClient.get<CurrentUserDto>("auth/me");
  return mapToUserModel(response.data);
}
