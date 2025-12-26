import { createContext, type ReactNode } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "react-oidc-context";
import { fetchCurrentUser, type CurrentUserDto, type PermissionDto } from "@/services/currentUser.service";

export interface PermissionsContextValue {
  user: CurrentUserDto | null;
  permissions: PermissionDto[];
  isLoading: boolean;
  error: Error | null;
}

export const PermissionsContext = createContext<PermissionsContextValue | undefined>(undefined);

export function PermissionsProvider({ children }: { children: ReactNode }) {
  const auth = useAuth();

  const { data: user, isLoading, error } = useQuery({
    queryKey: ["currentUser"],
    queryFn: fetchCurrentUser,
    enabled: auth.isAuthenticated,
    staleTime: 5 * 60 * 1000, // 5 minutes
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
