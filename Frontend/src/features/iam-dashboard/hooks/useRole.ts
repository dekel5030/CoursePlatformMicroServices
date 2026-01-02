import { useQuery } from "@tanstack/react-query";
import { fetchRoleByName } from "../api/Iam";
import type { RoleDetailDto } from "../types/RoleDetailDto";

export function useRole(roleName: string | undefined) {
  return useQuery<RoleDetailDto, Error>({
    queryKey: ["auth", "roles", roleName],
    queryFn: () => fetchRoleByName(roleName!),
    enabled: !!roleName,
  });
}
