import { useAuth } from "./useAuth";
import { hasPermission } from "../utils/permissionEvaluation";
import { ActionType, ResourceType, ResourceId } from "../types/AuthTypes";

/**
 * Type-safe permission check hook using AuthTypes enums.
 */
export function useHasPermission(
  action: ActionType,
  resource: ResourceType,
  resourceId?: ResourceId
): boolean;

/**
 * Legacy string-based permission check (backwards compatibility).
 * @deprecated Use the type-safe version with ActionType, ResourceType, and ResourceId instead.
 */
export function useHasPermission(
  action: string,
  resource: string,
  resourceId?: string
): boolean;

export function useHasPermission(
  action: ActionType | string,
  resource: ResourceType | string,
  resourceId: ResourceId | string = "*"
): boolean {
  const { permissions, isLoading } = useAuth();

  if (isLoading) return false;

  // Normalize resourceId
  const normalizedResourceId =
    typeof resourceId === "string" ? resourceId : resourceId.value;

  return hasPermission(permissions, action as any, resource as any, normalizedResourceId);
}
