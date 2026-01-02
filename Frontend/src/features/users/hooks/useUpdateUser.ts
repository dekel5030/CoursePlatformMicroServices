import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  updateUser,
  type UpdateUserRequest,
  type User,
} from "@/features/users/api";

export function useUpdateUser(userId: string) {
  const queryClient = useQueryClient();

  return useMutation<User, Error, UpdateUserRequest>({
    mutationFn: (data: UpdateUserRequest) => updateUser(userId, data),
    onSuccess: (updatedUser) => {
      queryClient.setQueryData(["users", userId], updatedUser);
    },
  });
}
