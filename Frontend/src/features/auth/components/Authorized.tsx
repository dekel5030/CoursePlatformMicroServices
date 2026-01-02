import React from "react";
import { useHasPermission } from "../hooks/useHasPermission";
import { useAuth } from "../hooks/useAuth";

interface AuthorizedProps {
  children: React.ReactNode;
  fallback?: React.ReactNode;
  action: string;
  resource: string;
  resourceId?: string;
}

export const Authorized: React.FC<AuthorizedProps> = ({
  children,
  fallback = null,
  action,
  resource,
  resourceId = "*",
}) => {
  const { isAuthenticated } = useAuth();

  const isAuthorized = useHasPermission(action, resource, resourceId);

  if (!isAuthenticated || !isAuthorized) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};
