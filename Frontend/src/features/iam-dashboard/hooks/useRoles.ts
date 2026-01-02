import { useQuery } from "@tanstack/react-query";
import { fetchAllRoles } from "../api/Iam";
import type { RoleListItemDto } from "../types/RoleListItemDto";

export function useRoles() {
  return useQuery<RoleListItemDto[], Error>({
    queryKey: ["auth", "roles"],
    queryFn: fetchAllRoles,
  });
}
