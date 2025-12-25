import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  addUserRole,
  removeUserRole,
  addUserPermission,
  removeUserPermission,
} from '../services/authManagement.service';
import type { AddPermissionRequest, AddRoleRequest } from '../types';

export function useUserManagement(userId: string) {
  const queryClient = useQueryClient();

  const addRole = useMutation({
    mutationFn: (request: AddRoleRequest) => addUserRole(userId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
    },
  });

  const removeRole = useMutation({
    mutationFn: (roleName: string) => removeUserRole(userId, roleName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
    },
  });

  const addPermission = useMutation({
    mutationFn: (permission: AddPermissionRequest) =>
      addUserPermission(userId, permission),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeUserPermission(userId, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
    },
  });

  return {
    addRole,
    removeRole,
    addPermission,
    removePermission,
  };
}
