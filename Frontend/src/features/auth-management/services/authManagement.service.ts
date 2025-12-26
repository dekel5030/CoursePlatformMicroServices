import { axiosClient } from "@/api/axiosClient";
import type {
  RoleListItemDto,
  RoleDetailDto,
  UserDto,
  AddPermissionRequest,
  AddRoleRequest,
  PaginatedResponse,
} from "../types";

export async function fetchAllRoles(): Promise<RoleListItemDto[]> {
  const response = await axiosClient.get<RoleListItemDto[]>("admin/roles");
  return response.data;
}

export async function fetchRoleByName(
  roleName: string
): Promise<RoleDetailDto> {
  const response = await axiosClient.get<RoleDetailDto>(
    `admin/roles/${encodeURIComponent(roleName)}`
  );
  return response.data;
}

export async function addRolePermission(
  roleName: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(
    `admin/roles/${encodeURIComponent(roleName)}/permissions`,
    permission
  );
}

export async function removeRolePermission(
  roleName: string,
  permissionKey: string
): Promise<void> {
  await axiosClient.delete(
    `admin/roles/${encodeURIComponent(
      roleName
    )}/permissions/${encodeURIComponent(permissionKey)}`
  );
}

export async function fetchAllUsers(): Promise<UserDto[]> {
  const response = await axiosClient.get<UserDto[]>("admin/users");
  return response.data;
}

export async function fetchUserById(userId: string): Promise<UserDto> {
  const response = await axiosClient.get<UserDto>(`admin/users/${userId}`);
  return response.data;
}

export async function addUserRole(
  userId: string,
  request: AddRoleRequest
): Promise<void> {
  await axiosClient.post(`admin/users/${userId}/roles`, request);
}

export async function removeUserRole(
  userId: string,
  roleName: string
): Promise<void> {
  await axiosClient.delete(
    `admin/users/${userId}/roles/${encodeURIComponent(roleName)}`
  );
}

export async function addUserPermission(
  userId: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(`admin/users/${userId}/permissions`, permission);
}

export async function removeUserPermission(
  userId: string,
  permissionKey: string
): Promise<void> {
  await axiosClient.delete(
    `admin/users/${userId}/permissions/${encodeURIComponent(permissionKey)}`
  );
}
