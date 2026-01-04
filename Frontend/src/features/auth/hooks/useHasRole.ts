import { useAuth } from "./useAuth";

export function useHasRole(roles: string | string[]): boolean {
  const { user, isLoading } = useAuth();

  if (isLoading || !user) return false;

  const rolesToCheck = (Array.isArray(roles) ? roles : [roles]).map((r) =>
    r.toLowerCase()
  );

  const userRoleNames = user.roles.map((role) => role.name.toLowerCase());

  return rolesToCheck.some((role) => userRoleNames.includes(role));
}
