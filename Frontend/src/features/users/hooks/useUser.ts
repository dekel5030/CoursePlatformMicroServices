import { useQuery } from '@tanstack/react-query';
import { fetchUserById, type User } from '@/services/UsersAPI';

export function useUser(id: string | undefined) {
  return useQuery<User, Error>({
    queryKey: ['users', id],
    queryFn: () => fetchUserById(id!),
    enabled: !!id, 
  });
}
