// Pages
export { default as UserProfilePage } from "./pages/UserProfilePage";

// Components
export * from "./components";

// Hooks - Consolidated data access with centralized query keys
export { useUser, useUpdateUser, usersQueryKeys } from "./hooks/use-users";

// API
export * from "./api";

// Types
export type { User } from "./api";
