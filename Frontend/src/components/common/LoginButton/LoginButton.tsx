import { useAuth } from "@/hooks";

export function LoginButton() {
  const auth = useAuth();

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
        <button onClick={() => auth.logout()}>Log out</button>
      </div>
    );
  }

  return <button onClick={() => auth.login()}>Log in with Keycloak</button>;
}
