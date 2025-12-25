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

export async function getUserById(userId: string): Promise<UserDto> {
  return apiClient.get<UserDto>(`/auth/users/${userId}`);
}

export async function getAllRoles(): Promise<RoleListDto[]> {
  return apiClient.get<RoleListDto[]>('/auth/roles');
}

export async function getRoleByName(roleName: string): Promise<RoleDetailDto> {
  return apiClient.get<RoleDetailDto>(`/auth/roles/${roleName}`);
}

export async function createRole(request: CreateRoleRequest): Promise<CreateRoleResponse> {
  return apiClient.post<CreateRoleResponse>('/auth/roles', request);
}

export async function addRolePermission(
  roleName: string,
  request: RoleAddPermissionRequest
): Promise<void> {
  return apiClient.post<void>(`/auth/roles/${roleName}/permissions`, request);
}

export async function removeRolePermission(
  roleName: string,
  permissionKey: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/roles/${roleName}/permissions/${permissionKey}`);
}

export async function addUserRole(
  userId: string,
  request: UserAddRoleRequest
): Promise<void> {
  return apiClient.post<void>(`/auth/users/${userId}/roles`, request);
}

export async function removeUserRole(
  userId: string,
  roleName: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/users/${userId}/roles/${roleName}`);
}

export async function addUserPermission(
  userId: string,
  request: UserAddPermissionRequest
): Promise<void> {
  return apiClient.post<void>(`/auth/users/${userId}/permissions`, request);
}

export async function removeUserPermission(
  userId: string,
  permissionKey: string
): Promise<void> {
  return apiClient.delete<void>(`/auth/users/${userId}/permissions/${permissionKey}`);
}
