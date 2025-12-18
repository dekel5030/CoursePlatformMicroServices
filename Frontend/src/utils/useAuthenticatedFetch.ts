import { useAuth } from "react-oidc-context";
import { useCallback } from "react";

export function useAuthenticatedFetch() {
  const auth = useAuth();

  const token = auth.user?.access_token;

  const authenticatedFetch = useCallback(
    async (input: RequestInfo | URL, init?: RequestInit): Promise<Response> => {
      const headers = new Headers(init?.headers);

      if (token) {
        headers.set("Authorization", `Bearer ${token}`);
      }

      const response = await fetch(input, {
        ...init,
        headers,
      });

      return response;
    },
    [token]
  );

  return authenticatedFetch;
}
