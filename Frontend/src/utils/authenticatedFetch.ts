/**
 * Authenticated Fetch Wrapper
 * 
 * This module provides a reusable fetch wrapper that automatically handles
 * access token expiration by intercepting 401 Unauthorized responses,
 * refreshing the access token, and retrying the original request.
 * 
 * Features:
 * - Automatic 401 handling and token refresh
 * - Single-flight refresh (prevents concurrent duplicate refreshes)
 * - Automatic logout on refresh failure
 * - Full TypeScript support
 * - Seamless integration with AuthContext
 * 
 * Usage:
 * ```typescript
 * import { createAuthenticatedFetch } from './utils/authenticatedFetch';
 * 
 * // In your component or service:
 * const authFetch = createAuthenticatedFetch(
 *   () => currentUser?.accessToken,
 *   refreshAccessToken,
 *   logout
 * );
 * 
 * // Use like normal fetch:
 * const response = await authFetch('/api/users/123', {
 *   method: 'GET',
 *   headers: { 'Content-Type': 'application/json' }
 * });
 * ```
 */

/**
 * Type for the access token getter function
 */
type AccessTokenGetter = () => string | undefined | null;

/**
 * Type for the token refresh function
 */
type TokenRefresher = () => Promise<string>;

/**
 * Type for the logout function
 */
type LogoutFunction = () => Promise<void>;

/**
 * State to manage ongoing token refresh attempts
 */
let refreshPromise: Promise<string> | null = null;

/**
 * Creates an authenticated fetch function that automatically handles token refresh on 401 responses.
 * 
 * @param getAccessToken - Function that returns the current access token
 * @param refreshToken - Function that refreshes the access token
 * @param logout - Function that logs out the user
 * @returns A fetch function that automatically handles 401 responses
 */
export function createAuthenticatedFetch(
  getAccessToken: AccessTokenGetter,
  refreshToken: TokenRefresher,
  logout: LogoutFunction
): typeof fetch {
  /**
   * Performs a token refresh, ensuring only one refresh happens at a time
   * (single-flight pattern)
   */
  const performRefresh = async (): Promise<string> => {
    // If a refresh is already in progress, wait for it
    if (refreshPromise) {
      return refreshPromise;
    }

    // Start a new refresh
    refreshPromise = refreshToken()
      .then((newToken) => {
        refreshPromise = null;
        return newToken;
      })
      .catch((error) => {
        refreshPromise = null;
        throw error;
      });

    return refreshPromise;
  };

  /**
   * The authenticated fetch wrapper
   */
  return async (input: RequestInfo | URL, init?: RequestInit): Promise<Response> => {
    // Get the current access token
    const token = getAccessToken();

    // Prepare headers with authorization token
    const headers = new Headers(init?.headers);
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }

    // Make the initial request
    let response = await fetch(input, {
      ...init,
      headers,
    });

    // If we get a 401, attempt to refresh the token and retry
    if (response.status === 401) {
      try {
        // Refresh the access token (single-flight)
        const newToken = await performRefresh();

        // Update the Authorization header with the new token
        headers.set('Authorization', `Bearer ${newToken}`);

        // Retry the original request with the new token
        response = await fetch(input, {
          ...init,
          headers,
        });
      } catch (error) {
        // Log the error for debugging
        console.error('Token refresh failed:', error);
        // If refresh fails, logout the user
        await logout();
        throw new Error('Session expired. Please login again.');
      }
    }

    return response;
  };
}

/**
 * Hook-style factory for creating authenticated fetch in React components
 * 
 * This is a convenience function that can be used directly with the AuthContext.
 * 
 * @param currentUser - The current authenticated user (with accessToken)
 * @param refreshAccessToken - Function to refresh the access token
 * @param logout - Function to logout the user
 * @returns An authenticated fetch function
 */
export function useAuthenticatedFetch(
  currentUser: { accessToken: string } | null,
  refreshAccessToken: TokenRefresher,
  logout: LogoutFunction
): typeof fetch {
  return createAuthenticatedFetch(
    () => currentUser?.accessToken,
    refreshAccessToken,
    logout
  );
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
 * @param getAccessToken - Function that returns the current access token
 * @param refreshToken - Function that refreshes the access token
 * @param logout - Function that logs out the user
 */
export function initializeAuthenticatedFetch(
  getAccessToken: AccessTokenGetter,
  refreshToken: TokenRefresher,
  logout: LogoutFunction
): void {
  globalAuthFetch = createAuthenticatedFetch(getAccessToken, refreshToken, logout);
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
