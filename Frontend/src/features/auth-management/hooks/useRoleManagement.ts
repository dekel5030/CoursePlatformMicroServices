import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  addRolePermission,
  removeRolePermission,
} from '../services/authManagement.service';
import type { AddPermissionRequest } from '../types';

export function useRoleManagement(roleName: string) {
  const queryClient = useQueryClient();

  const addPermission = useMutation({
    mutationFn: (permission: AddPermissionRequest) =>
      addRolePermission(roleName, permission),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'roles'] });
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeRolePermission(roleName, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'roles'] });
    },
  });

  return {
    addPermission,
    removePermission,
  };
}
