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
} from "../api/Iam";
import type { UserDto } from "../types/UserDto";
import type { RoleDetailDto } from "../types/RoleDetailDto";
import type { RoleListItemDto } from "../types/RoleListItemDto";
import type { AddRoleRequest } from "../types/AddRoleRequest";
import type { AddPermissionRequest } from "../types/AddPermissionRequest";

// Centralized Query Keys
export const iamQueryKeys = {
  all: ["auth"] as const,
  users: {
    all: () => [...iamQueryKeys.all, "users"] as const,
    detail: (userId: string) => [...iamQueryKeys.users.all(), userId] as const,
  },
  roles: {
    all: () => [...iamQueryKeys.all, "roles"] as const,
    detail: (roleName: string) => [...iamQueryKeys.roles.all(), roleName] as const,
  },
} as const;

// User Queries
export function useUsers() {
  return useQuery({
    queryKey: iamQueryKeys.users.all(),
    queryFn: () => fetchAllUsers(),
  });
}

export function useUser(userId: string | undefined) {
  return useQuery<UserDto, Error>({
    queryKey: userId ? iamQueryKeys.users.detail(userId) : ["auth", "users", "undefined"],
    queryFn: () => fetchUserById(userId!),
    enabled: !!userId,
  });
}

// User Mutations
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

// Role Queries
export function useRoles() {
  return useQuery<RoleListItemDto[], Error>({
    queryKey: iamQueryKeys.roles.all(),
    queryFn: fetchAllRoles,
  });
}

export function useRole(roleName: string | undefined) {
  return useQuery<RoleDetailDto, Error>({
    queryKey: roleName ? iamQueryKeys.roles.detail(roleName) : ["auth", "roles", "undefined"],
    queryFn: () => fetchRoleByName(roleName!),
    enabled: !!roleName,
  });
}

// Role Mutations
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
