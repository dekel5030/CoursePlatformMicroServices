import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
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
      toast.success('Permission added to role successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to add permission to role');
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeRolePermission(roleName, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'roles'] });
      toast.success('Permission removed from role successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to remove permission from role');
    },
  });

  return {
    addPermission,
    removePermission,
  };
}
