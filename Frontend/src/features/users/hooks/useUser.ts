import { useQuery } from '@tanstack/react-query';
import { fetchUserById, type User } from '@/services/UsersAPI';

/**
 * Hook to fetch a user by ID
 */
export function useUser(id: string | undefined) {
  return useQuery<User, Error>({
    queryKey: ['users', id],
    queryFn: () => fetchUserById(id!),
    enabled: !!id, // Only run query if id exists
  });
}
