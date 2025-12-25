import { AuthProvider as OidcProvider, useAuth as useOidcAuth } from "react-oidc-context";
import { useEffect, type ReactNode } from "react";
import { setTokenProvider } from "@/api";
import { keycloakConfig } from "@/config/app.config";
import { AuthContext, type AuthContextValue } from "@/hooks/useAuth";

const oidcConfig = {
  authority: `${keycloakConfig.url}/realms/${keycloakConfig.realm}`,
  client_id: keycloakConfig.clientId,
  redirect_uri: keycloakConfig.redirectUri,
  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,
  onSigninCallback: () => {
    window.history.replaceState({}, document.title, window.location.pathname);
  },
};

function AuthContextProvider({ children }: { children: ReactNode }) {
  const auth = useOidcAuth();

  useEffect(() => {
    setTokenProvider(() => auth.user?.access_token);
  }, [auth.user?.access_token]);

  const value: AuthContextValue = {
    user: auth.user ? {
      profile: auth.user.profile,
      access_token: auth.user.access_token,
    } : null,
    isAuthenticated: auth.isAuthenticated,
    isLoading: auth.isLoading,
    error: auth.error || null,
    login: () => auth.signinRedirect(),
    logout: () => auth.removeUser(),
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  return (
    <OidcProvider {...oidcConfig}>
      <AuthContextProvider>{children}</AuthContextProvider>
    </OidcProvider>
  );
}
