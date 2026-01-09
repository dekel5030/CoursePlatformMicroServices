export const AuthRoutes = {
  forbidden: () => "/forbidden",
  userProfile: (userId: string) => `/users/${userId}`,
} as const;

export const AuthResources = {
  USER_RESOURCE_TYPE: "User" as const,
} as const;
