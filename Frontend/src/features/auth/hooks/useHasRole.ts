import { useAuth } from "./useAuth";

/**
 * Hook to check if the current user has one or more specific roles.
 * Returns true if the user has at least one of the specified roles.
 * 
 * @param roles - Single role or array of roles to check
 * @returns boolean indicating if user has any of the specified roles
 * 
 * @example
 * const isAdmin = useHasRole("Admin");
 * const isStaff = useHasRole(["Admin", "Instructor"]);
 */
export function useHasRole(roles: string | string[]): boolean {
  const { user, isLoading } = useAuth();

  if (isLoading || !user) return false;

  const rolesToCheck = Array.isArray(roles) ? roles : [roles];
  const userRoleNames = user.roles.map((role) => role.name);

  return rolesToCheck.some((role) => userRoleNames.includes(role));
}
