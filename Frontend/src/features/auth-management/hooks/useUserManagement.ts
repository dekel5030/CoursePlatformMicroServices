import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
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
      queryClient.invalidateQueries({ queryKey: ['auth', 'users'] });
      toast.success('Role added successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to add role');
    },
  });

  const removeRole = useMutation({
    mutationFn: (roleName: string) => removeUserRole(userId, roleName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
      queryClient.invalidateQueries({ queryKey: ['auth', 'users'] });
      toast.success('Role removed successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to remove role');
    },
  });

  const addPermission = useMutation({
    mutationFn: (permission: AddPermissionRequest) =>
      addUserPermission(userId, permission),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
      queryClient.invalidateQueries({ queryKey: ['auth', 'users'] });
      toast.success('Permission added successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to add permission');
    },
  });

  const removePermission = useMutation({
    mutationFn: (permissionKey: string) =>
      removeUserPermission(userId, permissionKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auth', 'users', userId] });
      queryClient.invalidateQueries({ queryKey: ['auth', 'users'] });
      toast.success('Permission removed successfully');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to remove permission');
    },
  });

  return {
    addRole,
    removeRole,
    addPermission,
    removePermission,
  };
}
