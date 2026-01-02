import { useAuth } from "./useAuth";
import { hasPermission } from "../utils/permissionEvaluation";

export function useHasPermission(
  action: string,
  resource: string,
  resourceId: string = "*"
): boolean {
  const { permissions, isLoading } = useAuth();

  if (isLoading) return false;

  return hasPermission(permissions, action, resource, resourceId);
}
