/**
 * React Query key factory for users
 */
export const usersQueryKeys = {
  all: ["users"] as const,
  detail: (id: string) => [...usersQueryKeys.all, id] as const,
} as const;
