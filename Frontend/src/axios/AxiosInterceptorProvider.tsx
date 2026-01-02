import { useEffect, type ReactNode } from "react";
import { useAuth } from "react-oidc-context";
import { useQueryClient } from "@tanstack/react-query";
import { setLogoutCallback } from "@/axios/axiosClient";

export function AxiosInterceptorProvider({
  children,
}: {
  children: ReactNode;
}) {
  const auth = useAuth();
  const queryClient = useQueryClient();

  useEffect(() => {
    const handleLogout = () => {
      queryClient.clear();

      void auth.removeUser();
    };

    setLogoutCallback(handleLogout);

    return () => {
      setLogoutCallback(() => {});
    };
  }, [auth, queryClient]);

  return <>{children}</>;
}
