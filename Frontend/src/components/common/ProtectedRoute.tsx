import type { ReactNode } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks";
import { useHasRole } from "@/features/auth/hooks";

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRoles?: string[];
  redirectTo?: string;
}

/**
 * Component that protects routes based on user roles.
 * Redirects unauthorized users to the specified path or login page.
 */
export default function ProtectedRoute({
  children,
  requiredRoles = [],
  redirectTo = "/forbidden",
}: ProtectedRouteProps) {
  const { isAuthenticated, isLoading } = useAuth();
  const hasRequiredRole = useHasRole(requiredRoles);
  const location = useLocation();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center space-y-4">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto" />
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </div>
    );
  }

  // Redirect to login if not authenticated, preserving the intended destination
  if (!isAuthenticated) {
    const redirectUrl = encodeURIComponent(location.pathname + location.search);
    return <Navigate to={`/login?redirect=${redirectUrl}`} replace />;
  }

  // Redirect if user doesn't have required role
  if (requiredRoles.length > 0 && !hasRequiredRole) {
    return <Navigate to={redirectTo} replace />;
  }

  return <>{children}</>;
}
