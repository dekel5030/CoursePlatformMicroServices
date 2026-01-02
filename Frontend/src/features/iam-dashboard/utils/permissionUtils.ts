import type { PermissionDto } from "../types/PermissionDto";

export function groupPermissionsByCategory(permissions: PermissionDto[]) {
  const grouped: Record<string, PermissionDto[]> = {};

  permissions.forEach((permission) => {
    const category = permission.resource || "Other";
    if (!grouped[category]) {
      grouped[category] = [];
    }
    grouped[category].push(permission);
  });

  return grouped;
}
