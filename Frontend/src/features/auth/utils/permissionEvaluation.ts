import type { PermissionDto } from "../types/PermissionDto";

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
