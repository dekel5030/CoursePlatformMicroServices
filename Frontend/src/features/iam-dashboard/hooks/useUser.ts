import { useQuery } from "@tanstack/react-query";
import { fetchUserById } from "@/features/iam-dashboard/api/Iam";
import type { UserDto } from "@/features/iam-dashboard/types/UserDto";

export function useUser(userId: string | undefined) {
  return useQuery<UserDto, Error>({
    queryKey: ["auth", "users", userId],
    queryFn: () => fetchUserById(userId!),
    enabled: !!userId,
  });
}
