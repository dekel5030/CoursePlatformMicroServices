import { AuthProvider as OidcProvider } from "react-oidc-context";
import type { ReactNode } from "react";

const oidcConfig = {
  authority: "http://localhost:8080/realms/course-platform",
  client_id: "frontend",
  redirect_uri: "http://localhost:5067",

  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,

  onSigninCallback: () => {
    window.history.replaceState({}, document.title, window.location.pathname);
  },
};

export function AuthProvider({ children }: { children: ReactNode }) {
  return <OidcProvider {...oidcConfig}>{children}</OidcProvider>;
}
