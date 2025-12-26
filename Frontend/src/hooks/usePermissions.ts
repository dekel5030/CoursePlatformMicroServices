import { useContext } from "react";
import { PermissionsContext, type PermissionsContextValue } from "@/contexts/PermissionsContext";
import type { CurrentUserDto, PermissionDto } from "@/services/currentUser.service";

/**
 * Hook to access the permissions context
 */
export function usePermissionsContext(): PermissionsContextValue {
  const context = useContext(PermissionsContext);
  if (context === undefined) {
    throw new Error("usePermissionsContext must be used within a PermissionsProvider");
  }
  return context;
}

/**
 * Hook to access the current authenticated user's data
 */
export function useCurrentUser(): CurrentUserDto | null {
  const { user } = usePermissionsContext();
  return user;
}

/**
 * Hook to access the current user's permissions
 */
export function usePermissions(): PermissionDto[] {
  const { permissions } = usePermissionsContext();
  return permissions;
}

/**
 * Hook to check if the user has a specific permission
 * @param action - The action to check (e.g., "Create", "Read", "Update", "Delete")
 * @param resource - The resource to check (e.g., "Course", "User", "Enrollment")
 * @param resourceId - Optional specific resource ID (defaults to "*" for any)
 * @returns true if the user has the permission with "Allow" effect
 */
export function useHasPermission(
  action: string,
  resource: string,
  resourceId: string = "*"
): boolean {
  const { permissions } = usePermissionsContext();

  return permissions.some(
    (p) =>
      p.action.toLowerCase() === action.toLowerCase() &&
      p.resource.toLowerCase() === resource.toLowerCase() &&
      (p.resourceId === resourceId || p.resourceId === "*") &&
      p.effect.toLowerCase() === "allow"
  );
}

/**
 * Hook to check if the user has any of the specified permissions
 * @param checks - Array of permission checks [action, resource, resourceId?]
 * @returns true if the user has at least one of the specified permissions
 */
export function useHasAnyPermission(
  checks: Array<[string, string, string?]>
): boolean {
  const { permissions } = usePermissionsContext();

  return checks.some(([action, resource, resourceId = "*"]) =>
    permissions.some(
      (p) =>
        p.action.toLowerCase() === action.toLowerCase() &&
        p.resource.toLowerCase() === resource.toLowerCase() &&
        (p.resourceId === resourceId || p.resourceId === "*") &&
        p.effect.toLowerCase() === "allow"
    )
  );
}

/**
 * Hook to check if the user has all of the specified permissions
 * @param checks - Array of permission checks [action, resource, resourceId?]
 * @returns true if the user has all of the specified permissions
 */
export function useHasAllPermissions(
  checks: Array<[string, string, string?]>
): boolean {
  const { permissions } = usePermissionsContext();

  return checks.every(([action, resource, resourceId = "*"]) =>
    permissions.some(
      (p) =>
        p.action.toLowerCase() === action.toLowerCase() &&
        p.resource.toLowerCase() === resource.toLowerCase() &&
        (p.resourceId === resourceId || p.resourceId === "*") &&
        p.effect.toLowerCase() === "allow"
    )
  );
}

/**
 * Hook to check if the user has a specific role
 * @param roleName - The role name to check
 * @returns true if the user has the specified role
 */
export function useHasRole(roleName: string): boolean {
  const { user } = usePermissionsContext();
  
  if (!user) {
    return false;
  }

  return user.roles.some(
    (role) => role.name.toLowerCase() === roleName.toLowerCase()
  );
}

/**
 * Hook to check if the user has any of the specified roles
 * @param roleNames - Array of role names to check
 * @returns true if the user has at least one of the specified roles
 */
export function useHasAnyRole(roleNames: string[]): boolean {
  const { user } = usePermissionsContext();
  
  if (!user) {
    return false;
  }

  return roleNames.some((roleName) =>
    user.roles.some(
      (role) => role.name.toLowerCase() === roleName.toLowerCase()
    )
  );
}
