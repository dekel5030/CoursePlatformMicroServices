/**
 * Centralized constants for auth routes and resource identifiers
 * Ensures consistency across components, hooks, and routing
 */

/**
 * Route path builders for auth
 */
export const AuthRoutes = {
  /**
   * Get the path to the forbidden page
   */
  forbidden: () => "/forbidden",

  /**
   * Get the path to a user profile
   */
  userProfile: (userId: string) => `/users/${userId}`,
} as const;

/**
 * Resource identifiers for permission checks
 */
export const AuthResources = {
  USER_RESOURCE_TYPE: "User" as const,
} as const;
