import type { ReactNode } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "react-oidc-context";
import { fetchCurrentUser } from "@/services/currentUser.service";
import {
  PermissionsContext,
  type PermissionsContextValue,
} from "@/contexts/PermissionsContext";

export function PermissionsProvider({ children }: { children: ReactNode }) {
  const auth = useAuth();

  const accessToken = auth.user?.access_token;

  const {
    data: user,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["currentUser", accessToken],
    queryFn: fetchCurrentUser,
    enabled: auth.isAuthenticated,
    staleTime: Infinity,
    retry: 1,
  });

  const permissions = user?.permissions ?? [];

  const value: PermissionsContextValue = {
    user: user ?? null,
    permissions,
    isLoading,
    error: error as Error | null,
  };

  return (
    <PermissionsContext.Provider value={value}>
      {children}
    </PermissionsContext.Provider>
  );
}
