/**
 * Authenticated Fetch Wrapper
 *
 * This module provides a simple fetch wrapper that:
 * - Ensures credentials: 'include' for HTTP-only cookie authentication
 * - Handles 401 responses by triggering logout
 *
 * No client-side JWT token management - authentication is handled via
 * HTTP-only cookies managed by the server.
 */

/**
 * Type for the logout function
 */
type LogoutFunction = () => Promise<void>;

/**
 * Creates an authenticated fetch function that handles session-based authentication.
 *
 * @param logout - Function that logs out the user
 * @returns A fetch function that handles 401 responses
 */
export function createAuthenticatedFetch(logout: LogoutFunction): typeof fetch {
  return async (
    input: RequestInfo | URL,
    init?: RequestInit
  ): Promise<Response> => {
    // Make the request with credentials included for cookie-based auth
    const response = await fetch(input, {
      ...init,
      credentials: "include",
    });

    // If we get a 401, session is invalid - logout user
    if (response.status === 401) {
      console.error("Session expired or unauthorized");
      await logout();
      throw new Error("Session expired. Please login again.");
    }

    return response;
  };
}

/**
 * Global authenticated fetch instance
 * This is initialized by the AuthProvider and can be used by service modules
 */
let globalAuthFetch: typeof fetch | null = null;

/**
 * Initializes the global authenticated fetch instance
 * This should be called by the AuthProvider when it mounts
 *
 * @param logout - Function that logs out the user
 */
export function initializeAuthenticatedFetch(logout: LogoutFunction): void {
  globalAuthFetch = createAuthenticatedFetch(logout);
}

/**
 * Gets the global authenticated fetch instance
 * Falls back to regular fetch if not initialized (e.g., for unauthenticated requests)
 *
 * Note: This is a lightweight getter that returns a cached instance. Calling it
 * multiple times has minimal performance impact.
 *
 * @returns The authenticated fetch function or regular fetch
 */
export function getAuthenticatedFetch(): typeof fetch {
  return globalAuthFetch || fetch;
}
