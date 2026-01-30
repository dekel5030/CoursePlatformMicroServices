import { useQuery } from "@tanstack/react-query";
import { fetchCurrentUser } from "../api";
import type { UserModel } from "../types/UserModel";
import { authQueryKeys } from "../query-keys";

/**
 * Fetch the current authenticated user
 */
export function useCurrentUser(token?: string) {
  return useQuery<UserModel, Error>({
    queryKey: authQueryKeys.currentUser(token),
    queryFn: fetchCurrentUser,
    enabled: !!token,
    staleTime: Infinity,
    retry: 1,
  });
}
