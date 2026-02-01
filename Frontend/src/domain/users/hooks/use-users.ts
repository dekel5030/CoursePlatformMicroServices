import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchUserById, updateUser } from "../api";
import type { User, UpdateUserRequest } from "../types";
import { usersQueryKeys } from "../query-keys";

/**
 * Fetch a user by ID
 */
export function useUser(id: string | undefined) {
  return useQuery<User, Error>({
    queryKey: id ? usersQueryKeys.detail(id) : ["users", "undefined"],
    queryFn: () => fetchUserById(id!),
    enabled: !!id,
  });
}

/**
 * Update a user
 */
export function useUpdateUser(userId: string) {
  const queryClient = useQueryClient();

  return useMutation<User, Error, UpdateUserRequest>({
    mutationFn: (data: UpdateUserRequest) => updateUser(userId, data),
    onSuccess: (updatedUser) => {
      queryClient.setQueryData(usersQueryKeys.detail(userId), updatedUser);
    },
  });
}
