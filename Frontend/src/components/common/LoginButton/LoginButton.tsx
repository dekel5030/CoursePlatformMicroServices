import { useAuth } from "react-oidc-context";

export function LoginButton() {
  const auth = useAuth();

  switch (auth.activeNavigator) {
    case "signinSilent":
      return <div>Signing you in...</div>;
    case "signoutRedirect":
      return <div>Signing you out...</div>;
  }

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  if (auth.error) {
    return <div>Oops... {auth.error.message}</div>;
  }

  if (auth.isAuthenticated) {
    return (
      <div>
        Hello {auth.user?.profile.preferred_username}
        <button onClick={() => auth.removeUser()}>Log out</button>
      </div>
    );
  }

  return (
    <button onClick={() => auth.signinRedirect()}>Log in with Keycloak</button>
  );
}
