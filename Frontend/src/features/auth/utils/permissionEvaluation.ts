import type { PermissionDto } from "../types/PermissionDto";
import { ActionType, ResourceType, ResourceId } from "../types/AuthTypes";

/**
 * Type-safe permission evaluation using AuthTypes enums.
 * Mirrors the backend PermissionEvaluator logic.
 */
export function hasPermission(
  permissions: PermissionDto[],
  requiredAction: ActionType,
  requiredResource: ResourceType,
  requiredResourceId: ResourceId
): boolean;

/**
 * Legacy string-based permission evaluation (backwards compatibility).
 * @deprecated Use the type-safe version with ActionType, ResourceType, and ResourceId instead.
 */
export function hasPermission(
  permissions: PermissionDto[],
  requiredAction: string,
  requiredResource: string,
  requiredId: string
): boolean;

export function hasPermission(
  permissions: PermissionDto[],
  requiredAction: ActionType | string,
  requiredResource: ResourceType | string,
  requiredIdOrResourceId: ResourceId | string
): boolean {
  let isAllowed = false;

  // Normalize inputs to strings for comparison
  const targetAction =
    typeof requiredAction === "string"
      ? requiredAction.toLowerCase()
      : (requiredAction as string).toLowerCase();
  const targetResource =
    typeof requiredResource === "string"
      ? requiredResource.toLowerCase()
      : (requiredResource as string).toLowerCase();
  const targetId =
    typeof requiredIdOrResourceId === "string"
      ? requiredIdOrResourceId.toLowerCase()
      : requiredIdOrResourceId.value.toLowerCase();

  for (const permission of permissions) {
    const pAction = permission.action.toLowerCase();
    const pResource = permission.resource.toLowerCase();
    const pId = permission.resourceId.toLowerCase();
    const pEffect = permission.effect.toLowerCase();

    const actionMatch =
      pAction === "wildcard" || pAction === "*" || pAction === targetAction;
    const resourceMatch =
      pResource === "wildcard" ||
      pResource === "*" ||
      pResource === targetResource;
    const idMatch = pId === "wildcard" || pId === "*" || pId === targetId;

    if (actionMatch && resourceMatch && idMatch) {
      if (pEffect === "deny") {
        return false;
      }
      if (pEffect === "allow") {
        isAllowed = true;
      }
    }
  }

  return isAllowed;
}
