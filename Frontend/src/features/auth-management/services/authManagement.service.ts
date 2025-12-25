import { axiosClient } from '@/api/axiosClient';
import type {
  RoleListItemDto,
  RoleDetailDto,
  UserDto,
  AddPermissionRequest,
  AddRoleRequest,
} from '../types';

export async function fetchAllRoles(): Promise<RoleListItemDto[]> {
  const response = await axiosClient.get<RoleListItemDto[]>('/auth/roles');
  return response.data;
}

export async function fetchRoleByName(roleName: string): Promise<RoleDetailDto> {
  const response = await axiosClient.get<RoleDetailDto>(`/auth/roles/${encodeURIComponent(roleName)}`);
  return response.data;
}

export async function addRolePermission(
  roleName: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(`/auth/roles/${encodeURIComponent(roleName)}/permissions`, permission);
}

export async function removeRolePermission(
  roleName: string,
  permissionKey: string
): Promise<void> {
  await axiosClient.delete(
    `/auth/roles/${encodeURIComponent(roleName)}/permissions/${encodeURIComponent(permissionKey)}`
  );
}

export async function fetchUserById(userId: string): Promise<UserDto> {
  const response = await axiosClient.get<UserDto>(`/auth/users/${userId}`);
  return response.data;
}

export async function addUserRole(userId: string, request: AddRoleRequest): Promise<void> {
  await axiosClient.post(`/auth/users/${userId}/roles`, request);
}

export async function removeUserRole(userId: string, roleName: string): Promise<void> {
  await axiosClient.delete(`/auth/users/${userId}/roles/${encodeURIComponent(roleName)}`);
}

export async function addUserPermission(
  userId: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(`/auth/users/${userId}/permissions`, permission);
}

export async function removeUserPermission(
  userId: string,
  permissionKey: string
): Promise<void> {
  await axiosClient.delete(
    `/auth/users/${userId}/permissions/${encodeURIComponent(permissionKey)}`
  );
}
