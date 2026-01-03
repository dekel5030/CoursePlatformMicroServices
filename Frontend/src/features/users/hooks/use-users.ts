import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchUserById, updateUser } from "../api";
import type { User, UpdateUserRequest } from "../api";

// Centralized Query Keys
export const usersQueryKeys = {
  all: ["users"] as const,
  detail: (id: string) => [...usersQueryKeys.all, id] as const,
} as const;

// User Queries
export function useUser(id: string | undefined) {
  return useQuery<User, Error>({
    queryKey: id ? usersQueryKeys.detail(id) : ["users", "undefined"],
    queryFn: () => fetchUserById(id!),
    enabled: !!id,
  });
}

// User Mutations
export function useUpdateUser(userId: string) {
  const queryClient = useQueryClient();

  return useMutation<User, Error, UpdateUserRequest>({
    mutationFn: (data: UpdateUserRequest) => updateUser(userId, data),
    onSuccess: (updatedUser) => {
      queryClient.setQueryData(usersQueryKeys.detail(userId), updatedUser);
    },
  });
}
