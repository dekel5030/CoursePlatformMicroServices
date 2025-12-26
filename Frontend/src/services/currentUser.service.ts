import { axiosClient } from "@/api/axiosClient";

export interface PermissionDto {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}

export interface RoleDto {
  id: string;
  name: string;
}

export interface CurrentUserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: RoleDto[];
  permissions: PermissionDto[];
}

export async function fetchCurrentUser(): Promise<CurrentUserDto> {
  const response = await axiosClient.get<CurrentUserDto>("/me");
  return response.data;
}
