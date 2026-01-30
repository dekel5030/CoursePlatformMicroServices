import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  fetchAllUsers,
  fetchUserById,
  addUserRole,
  removeUserRole,
  addUserPermission,
  removeUserPermission,
  fetchAllRoles,
  fetchRoleByName,
  addRolePermission,
  removeRolePermission,
} from "../api";
import type { UserDto } from "../types/UserDto";
import type { RoleDetailDto } from "../types/RoleDetailDto";
import type { RoleListItemDto } from "../types/RoleListItemDto";
import type { AddRoleRequest } from "../types/AddRoleRequest";
import type { AddPermissionRequest } from "../types/AddPermissionRequest";
import { iamQueryKeys } from "../query-keys";

/**
 * Fetch all users
 */
export function useUsers() {
  return useQuery({
    queryKey: iamQueryKeys.users.all(),
    queryFn: () => fetchAllUsers(),
  });
}

/**
 * Fetch a user by ID
 */
export function useUser(userId: string | undefined) {
  return useQuery<UserDto, Error>({
    queryKey: userId ? iamQueryKeys.users.detail(userId) : ["auth", "users", "undefined"],
    queryFn: () => fetchUserById(userId!),
    enabled: !!userId,
  });
}

/**
 * User management mutations
 */
export function useUserManagement(userId: string) {
  const queryClient = useQueryClient();

  const addRole = useMutation({
    mutationFn: (request: AddRoleRequest) => addUserRole(userId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.detail(userId) });
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.all() });
      toast.success("Role added successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to add role");
    },
  });

  const removeRole = useMutation({
    mutationFn: (roleName: string) => removeUserRole(userId, roleName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.detail(userId) });
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.all() });
      toast.success("Role removed successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to remove role");
    },
  });

  const addPermission = useMutation({
    mutationFn: (permission: AddPermissionRequest) =>
      addUserPermission(userId, permission),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.detail(userId) });
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.all() });
      toast.success("Permission added successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to add permission");
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeUserPermission(userId, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.detail(userId) });
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.all() });
      toast.success("Permission removed successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to remove permission");
    },
  });

  return {
    addRole,
    removeRole,
    addPermission,
    removePermission,
  };
}

/**
 * Fetch all roles
 */
export function useRoles() {
  return useQuery<RoleListItemDto[], Error>({
    queryKey: iamQueryKeys.roles.all(),
    queryFn: fetchAllRoles,
  });
}

/**
 * Fetch a role by name
 */
export function useRole(roleName: string | undefined) {
  return useQuery<RoleDetailDto, Error>({
    queryKey: roleName ? iamQueryKeys.roles.detail(roleName) : ["auth", "roles", "undefined"],
    queryFn: () => fetchRoleByName(roleName!),
    enabled: !!roleName,
  });
}

/**
 * Role management mutations
 */
export function useRoleManagement(roleName: string) {
  const queryClient = useQueryClient();

  const addPermission = useMutation({
    mutationFn: (permission: AddPermissionRequest) =>
      addRolePermission(roleName, permission),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.roles.all() });
      toast.success("Permission added to role successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to add permission to role");
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeRolePermission(roleName, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: iamQueryKeys.roles.all() });
      toast.success("Permission removed from role successfully");
    },
    onError: (error: Error) => {
      toast.error(error.message || "Failed to remove permission from role");
    },
  });

  return {
    addPermission,
    removePermission,
  };
}
