import { createContext } from "react";
import type { CurrentUserDto } from "../types/CurrentUserDto";
import type { PermissionDto } from "../types/PermissionDto";

export interface AuthContextValue {
  user: CurrentUserDto | null;
  permissions: PermissionDto[];
  isAuthenticated: boolean;
  isLoading: boolean;
  error: Error | null;

  signoutRedirect: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined
);
