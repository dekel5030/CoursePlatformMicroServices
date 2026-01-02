import React from "react";
import { useHasPermission } from "../hooks/useHasPermission";
import { useAuth } from "../hooks/useAuth";
import { ActionType, ResourceType, ResourceId } from "../types/AuthTypes";

interface AuthorizedPropsTypeSafe {
  children: React.ReactNode;
  fallback?: React.ReactNode;
  action: ActionType;
  resource: ResourceType;
  resourceId?: ResourceId;
}

interface AuthorizedPropsLegacy {
  children: React.ReactNode;
  fallback?: React.ReactNode;
  action: string;
  resource: string;
  resourceId?: string;
}

type AuthorizedProps = AuthorizedPropsTypeSafe | AuthorizedPropsLegacy;

/**
 * Authorization wrapper component that conditionally renders children based on user permissions.
 * 
 * @example Type-safe usage (recommended):
 * ```tsx
 * <Authorized 
 *   action={ActionType.Create} 
 *   resource={ResourceType.Course}
 *   resourceId={ResourceId.Wildcard}
 * >
 *   <Button>Create Course</Button>
 * </Authorized>
 * ```
 * 
 * @example Legacy string-based usage (deprecated):
 * ```tsx
 * <Authorized action="create" resource="course" resourceId="*">
 *   <Button>Create Course</Button>
 * </Authorized>
 * ```
 */
export const Authorized: React.FC<AuthorizedProps> = ({
  children,
  fallback = null,
  action,
  resource,
  resourceId,
}) => {
  const { isAuthenticated } = useAuth();

  // Convert ResourceId to string if needed, default to wildcard
  const normalizedResourceId =
    resourceId === undefined
      ? ResourceId.Wildcard
      : typeof resourceId === "string"
        ? resourceId
        : resourceId;

  const isAuthorized = useHasPermission(
    action as any,
    resource as any,
    normalizedResourceId as any
  );

  if (!isAuthenticated || !isAuthorized) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};
