import { axiosClient } from "@/app/axios";
import type { AddRoleRequest } from "../types/AddRoleRequest";
import type { AddPermissionRequest } from "../types/AddPermissionRequest";
import type { UserDto } from "../types/UserDto";
import type { RoleDetailDto } from "../types/RoleDetailDto";
import type { RoleListItemDto } from "../types/RoleListItemDto";

/**
 * Fetch all roles
 */
export async function fetchAllRoles(): Promise<RoleListItemDto[]> {
  const response = await axiosClient.get<RoleListItemDto[]>("admin/roles");
  return response.data;
}

/**
 * Fetch a role by name
 */
export async function fetchRoleByName(
  roleName: string
): Promise<RoleDetailDto> {
  const response = await axiosClient.get<RoleDetailDto>(
    `admin/roles/${encodeURIComponent(roleName)}`
  );
  return response.data;
}

/**
 * Add a permission to a role
 */
export async function addRolePermission(
  roleName: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(
    `admin/roles/${encodeURIComponent(roleName)}/permissions`,
    permission
  );
}

/**
 * Remove a permission from a role
 */
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

/**
 * Fetch all users
 */
export async function fetchAllUsers(): Promise<UserDto[]> {
  const response = await axiosClient.get<UserDto[]>("admin/users");
  return response.data;
}

/**
 * Fetch a user by ID
 */
export async function fetchUserById(userId: string): Promise<UserDto> {
  const response = await axiosClient.get<UserDto>(`admin/users/${userId}`);
  return response.data;
}

/**
 * Add a role to a user
 */
export async function addUserRole(
  userId: string,
  request: AddRoleRequest
): Promise<void> {
  await axiosClient.post(`admin/users/${userId}/roles`, request);
}

/**
 * Remove a role from a user
 */
export async function removeUserRole(
  userId: string,
  roleName: string
): Promise<void> {
  await axiosClient.delete(
    `admin/users/${userId}/roles/${encodeURIComponent(roleName)}`
  );
}

/**
 * Add a permission to a user
 */
export async function addUserPermission(
  userId: string,
  permission: AddPermissionRequest
): Promise<void> {
  await axiosClient.post(`admin/users/${userId}/permissions`, permission);
}

/**
 * Remove a permission from a user
 */
export async function removeUserPermission(
  userId: string,
  permissionKey: string
): Promise<void> {
  await axiosClient.delete(
    `admin/users/${userId}/permissions/${encodeURIComponent(permissionKey)}`
  );
}
