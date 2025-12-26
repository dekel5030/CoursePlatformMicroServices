import { useQuery } from '@tanstack/react-query';
import { fetchAllUsers } from '../services/authManagement.service';

export function useUsers() {
  return useQuery({
    queryKey: ['auth', 'users'],
    queryFn: () => fetchAllUsers(),
  });
}
