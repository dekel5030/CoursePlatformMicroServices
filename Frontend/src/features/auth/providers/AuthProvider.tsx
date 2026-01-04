// auth/providers/AuthProvider.tsx
import { AuthProvider as OidcProvider, useAuth } from "react-oidc-context";
import { useQuery } from "@tanstack/react-query";
import type { ReactNode } from "react";
import { fetchCurrentUser } from "../api/currentUser";
import { AuthContext } from "../contexts/AuthContext";
import { getAndClearRedirectUrl } from "../utils/keycloakAuth";

const oidcConfig = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
  redirect_uri: window.location.origin,
  post_logout_redirect_uri: window.location.origin,
  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,
  loadUserInfo: true,
  onSigninCallback: () => {
    // After successful sign-in, redirect to the stored URL or default
    const redirectUrl = getAndClearRedirectUrl();
    window.history.replaceState({}, document.title, redirectUrl);
  },
};

function AuthStateProvider({ children }: { children: ReactNode }) {
  const oidc = useAuth(); // ה-Hook של react-oidc-context

  const {
    data: user,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["currentUser", oidc.user?.access_token],
    queryFn: fetchCurrentUser,
    enabled: oidc.isAuthenticated,
    staleTime: Infinity,
    retry: 1,
  });

  const value = {
    user: user ?? null,
    permissions: user?.permissions ?? [],
    isAuthenticated: oidc.isAuthenticated,
    isLoading: oidc.isLoading || isLoading,
    error: error as Error | null,
    signoutRedirect: () => oidc.signoutRedirect(),
    signinRedirect: () => oidc.signinRedirect(),
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  return (
    <OidcProvider {...oidcConfig}>
      <AuthStateProvider>{children}</AuthStateProvider>
    </OidcProvider>
  );
}
