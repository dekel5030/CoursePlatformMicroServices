import { useEffect, type ReactNode } from "react";
import { useAuth } from "react-oidc-context";
import { useQueryClient } from "@tanstack/react-query";
import { setLogoutCallback } from "@/api/axiosClient";

export function AxiosInterceptorProvider({ children }: { children: ReactNode }) {
  const auth = useAuth();
  const queryClient = useQueryClient();

  useEffect(() => {
    // Set up the logout callback for axios interceptor
    const handleLogout = () => {
      // Clear all cached queries
      queryClient.clear();
      
      // Remove OIDC user and trigger logout
      void auth.removeUser();
    };

    setLogoutCallback(handleLogout);

    return () => {
      setLogoutCallback(() => {});
    };
  }, [auth, queryClient]);

  return <>{children}</>;
}
