import { useMutation, useQueryClient } from '@tanstack/react-query';
import { updateUser, type UpdateUserRequest, type User } from '@/services/UsersAPI';

/**
 * Hook to update user profile
 */
export function useUpdateUser(userId: string) {
  const queryClient = useQueryClient();

  return useMutation<User, Error, UpdateUserRequest>({
    mutationFn: (data: UpdateUserRequest) => updateUser(userId, data),
    onSuccess: (updatedUser) => {
      // Invalidate and refetch user data
      queryClient.setQueryData(['users', userId], updatedUser);
      queryClient.invalidateQueries({ queryKey: ['users', userId] });
    },
  });
}
