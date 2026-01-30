/**
 * React Query key factory for IAM
 */
export const iamQueryKeys = {
  all: ["auth"] as const,
  users: {
    all: () => [...iamQueryKeys.all, "users"] as const,
    detail: (userId: string) => [...iamQueryKeys.users.all(), userId] as const,
  },
  roles: {
    all: () => [...iamQueryKeys.all, "roles"] as const,
    detail: (roleName: string) => [...iamQueryKeys.roles.all(), roleName] as const,
  },
} as const;
