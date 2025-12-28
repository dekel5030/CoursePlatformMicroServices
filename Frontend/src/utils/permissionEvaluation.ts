import type { PermissionDto } from "@/features/auth-management/types";

/**
 * Evaluates whether a user has permission to perform a specific action on a resource.
 * 
 * Logic:
 * 1. DENY overrides ALLOW. If any matching permission has "Deny", returns false immediately.
 * 2. Wildcards: 
 *    - 'Wildcard' or '*' matches ANY string in action/resource/resourceId.
 *    - Exact string match.
 * 
 * @param permissions - Array of PermissionDto objects.
 * @param requiredAction - The action to check (e.g., "Create").
 * @param requiredResource - The resource type (e.g., "Course").
 * @param requiredId - The resource ID (e.g., "*", "123").
 * @returns boolean - True if allowed, false otherwise.
 */
export function hasPermission(
  permissions: PermissionDto[],
  requiredAction: string,
  requiredResource: string,
  requiredId: string
): boolean {
  let isAllowed = false;

  const targetAction = requiredAction.toLowerCase();
  const targetResource = requiredResource.toLowerCase();
  const targetId = requiredId.toLowerCase();

  for (const permission of permissions) {
    const pAction = permission.action.toLowerCase();
    const pResource = permission.resource.toLowerCase();
    const pId = permission.resourceId.toLowerCase();
    const pEffect = permission.effect.toLowerCase();

    // Check matches
    const actionMatch = pAction === 'wildcard' || pAction === '*' || pAction === targetAction;
    const resourceMatch = pResource === 'wildcard' || pResource === '*' || pResource === targetResource;
    const idMatch = pId === 'wildcard' || pId === '*' || pId === targetId;

    if (actionMatch && resourceMatch && idMatch) {
      if (pEffect === 'deny') {
        // Deny overrides everything
        return false;
      }
      if (pEffect === 'allow') {
        isAllowed = true;
      }
    }
  }

  return isAllowed;
}