import { useQuery } from '@tanstack/react-query';
import { fetchUserById } from '../services/authManagement.service';
import type { UserDto } from '../types';

export function useAuthUser(userId: string | undefined) {
  return useQuery<UserDto, Error>({
    queryKey: ['auth', 'users', userId],
    queryFn: () => fetchUserById(userId!),
    enabled: !!userId,
  });
}
