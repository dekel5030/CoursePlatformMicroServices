import { type ReactNode } from "react";
import { useHasPermission, useHasAnyPermission } from "@/hooks";

interface PermissionGuardProps {
  action: string;
  resource: string;
  resourceId?: string;
  children: ReactNode;
  fallback?: ReactNode;
}

/**
 * Component that conditionally renders children based on user permissions
 */
export function PermissionGuard({
  action,
  resource,
  resourceId = "*",
  children,
  fallback = null,
}: PermissionGuardProps) {
  const hasPermission = useHasPermission(action, resource, resourceId);

  return hasPermission ? <>{children}</> : <>{fallback}</>;
}

interface AnyPermissionGuardProps {
  checks: Array<[string, string, string?]>;
  children: ReactNode;
  fallback?: ReactNode;
}

/**
 * Component that conditionally renders children if user has any of the specified permissions
 */
export function AnyPermissionGuard({
  checks,
  children,
  fallback = null,
}: AnyPermissionGuardProps) {
  const hasAnyPermission = useHasAnyPermission(checks);

  return hasAnyPermission ? <>{children}</> : <>{fallback}</>;
}
