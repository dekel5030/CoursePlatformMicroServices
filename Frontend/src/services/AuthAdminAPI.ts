import { apiClient } from "@/api";
import type {
  UserDto,
  RoleListDto,
  RoleDetailDto,
  CreateRoleRequest,
  CreateRoleResponse,
  UserAddRoleRequest,
  UserAddPermissionRequest,
  RoleAddPermissionRequest,
} from "@/types";

export async function getUserById(userId: string): Promise<UserDto> {
  return apiClient.get<UserDto>(`/admin/users/${userId}`);
}

export async function getAllRoles(): Promise<RoleListDto[]> {
  return apiClient.get<RoleListDto[]>("/admin/roles");
}

export async function getRoleByName(roleName: string): Promise<RoleDetailDto> {
  return apiClient.get<RoleDetailDto>(`/admin/roles/${roleName}`);
}

export async function createRole(
  request: CreateRoleRequest
): Promise<CreateRoleResponse> {
  return apiClient.post<CreateRoleResponse>("/admin/roles", request);
}

export async function addRolePermission(
  roleName: string,
  request: RoleAddPermissionRequest
): Promise<void> {
  return apiClient.post<void>(`/admin/roles/${roleName}/permissions`, request);
}

export async function removeRolePermission(
  roleName: string,
  permissionKey: string
): Promise<void> {
  return apiClient.delete<void>(
    `/admin/roles/${roleName}/permissions/${permissionKey}`
  );
}

export async function addUserRole(
  userId: string,
  request: UserAddRoleRequest
): Promise<void> {
  return apiClient.post<void>(`/admin/users/${userId}/roles`, request);
}

export async function removeUserRole(
  userId: string,
  roleName: string
): Promise<void> {
  return apiClient.delete<void>(`/admin/users/${userId}/roles/${roleName}`);
}

export async function addUserPermission(
  userId: string,
  request: UserAddPermissionRequest
): Promise<void> {
  return apiClient.post<void>(`/admin/users/${userId}/permissions`, request);
}

export async function removeUserPermission(
  userId: string,
  permissionKey: string
): Promise<void> {
  return apiClient.delete<void>(
    `/admin/users/${userId}/permissions/${permissionKey}`
  );
}
