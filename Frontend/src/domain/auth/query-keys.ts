/**
 * React Query key factory for auth
 */
export const authQueryKeys = {
  currentUser: (token?: string) => ["currentUser", token] as const,
} as const;
