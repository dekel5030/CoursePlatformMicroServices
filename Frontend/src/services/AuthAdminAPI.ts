import { apiClient } from '../lib/apiClient';
import type {
  UserDto,
  RoleListDto,
  RoleDetailDto,
  CreateRoleRequest,
  CreateRoleResponse,
  UserAddRoleRequest,
  UserAddPermissionRequest,
  RoleAddPermissionRequest,
} from '../types/auth';

export async function getUserById(userId: string, token?: string): Promise<UserDto> {
  return apiClient.get<UserDto>(`/auth/users/${userId}`, { token });
}

export async function getAllRoles(token?: string): Promise<RoleListDto[]> {
  return apiClient.get<RoleListDto[]>('/auth/roles', { token });
}

export async function getRoleByName(roleName: string, token?: string): Promise<RoleDetailDto> {
  return apiClient.get<RoleDetailDto>(`/auth/roles/${roleName}`, { token });
}

export async function createRole(request: CreateRoleRequest, token?: string): Promise<CreateRoleResponse> {
  return apiClient.post<CreateRoleResponse>('/auth/roles', request, { token });
}

export async function addRolePermission(
  roleName: string,
  request: RoleAddPermissionRequest,
  token?: string
): Promise<void> {
  return apiClient.post<void>(`/auth/roles/${roleName}/permissions`, request, { token });
}

export async function removeRolePermission(
  roleName: string,
  permissionKey: string,
  token?: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/roles/${roleName}/permissions/${permissionKey}`, { token });
}

export async function addUserRole(
  userId: string,
  request: UserAddRoleRequest,
  token?: string
): Promise<void> {
  return apiClient.post<void>(`/auth/users/${userId}/roles`, request, { token });
}

export async function removeUserRole(
  userId: string,
  roleName: string,
  token?: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/users/${userId}/roles/${roleName}`, { token });
}

export async function addUserPermission(
  userId: string,
  request: UserAddPermissionRequest,
  token?: string
): Promise<void> {
  return apiClient.post<void>(`/auth/users/${userId}/permissions`, request, { token });
}

export async function removeUserPermission(
  userId: string,
  permissionKey: string,
  token?: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/users/${userId}/permissions/${permissionKey}`, { token });
}
