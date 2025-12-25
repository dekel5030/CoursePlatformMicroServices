import { useQuery } from '@tanstack/react-query';
import { fetchAllRoles } from '../services/authManagement.service';
import type { RoleListItemDto } from '../types';

export function useRoles() {
  return useQuery<RoleListItemDto[], Error>({
    queryKey: ['auth', 'roles'],
    queryFn: fetchAllRoles,
  });
}
