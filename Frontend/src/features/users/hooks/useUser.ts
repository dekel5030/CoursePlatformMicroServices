import { useQuery } from "@tanstack/react-query";
import { fetchUserById, type User } from "@/features/users/api";

export function useUser(id: string | undefined) {
  return useQuery<User, Error>({
    queryKey: ["users", id],
    queryFn: () => fetchUserById(id!),
    enabled: !!id,
  });
}
