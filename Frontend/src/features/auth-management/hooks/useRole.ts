import { useQuery } from '@tanstack/react-query';
import { fetchRoleByName } from '../services/authManagement.service';
import type { RoleDetailDto } from '../types';

export function useRole(roleName: string | undefined) {
  return useQuery<RoleDetailDto, Error>({
    queryKey: ['auth', 'roles', roleName],
    queryFn: () => fetchRoleByName(roleName!),
    enabled: !!roleName,
  });
}
